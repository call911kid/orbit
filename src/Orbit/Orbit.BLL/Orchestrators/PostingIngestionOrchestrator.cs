using Orbit.BLL.Commands.JobPostings;
using Orbit.BLL.Interfaces;
using Orbit.BLL.Queries.ExcludedCompanies;
using Orbit.BLL.Queries.JobPostings;
using Orbit.Common.Enums;
using Orbit.DAL.Context;
using Orbit.Providers.Contracts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Orbit.BLL.Orchestrators;

public record PostingIngestionOrchestrator(
    Guid ProfileId,
    PostingSource Source = PostingSource.LinkedIn
) : IRequest;

public class PostingIngestionOrchestratorHandler : IRequestHandler<PostingIngestionOrchestrator>
{
    private readonly OrbitDbContext _context;
    private readonly IEnumerable<IPostingProvider> _providers;
    private readonly IMediator _mediator;
    private readonly IPostingAlertFormatter _formatter;
    private readonly ITelegramService _telegramService;
    private readonly IPostingMatcher _postingMatcher;

    public PostingIngestionOrchestratorHandler(
        OrbitDbContext context,
        IEnumerable<IPostingProvider> providers,
        IMediator mediator,
        IPostingAlertFormatter formatter,
        ITelegramService telegramService,
        IPostingMatcher postingMatcher)
    {
        _context = context;
        _providers = providers;
        _mediator = mediator;
        _formatter = formatter;
        _telegramService = telegramService;
        _postingMatcher = postingMatcher;
    }

    public async Task Handle(PostingIngestionOrchestrator request, CancellationToken cancellationToken)
    {
        var profile = await _context.SearchProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(sp => sp.Id == request.ProfileId, cancellationToken);

        if (profile is null || !profile.IsActive)
            return;

        var provider = _providers.FirstOrDefault(p => p.Source == request.Source);
        if (provider is null)
            return;

        var criteria = new PostingSearchCriteria(
            TargetTitles: profile.TargetJobTitles,
            TargetLocations: profile.TargetLocations,
            MaxPostingAge: TimeSpan.FromMinutes(profile.MaxPostingAgeMinutes),
            Workplace: profile.Workplace,
            Experience: profile.Experience,
            JobType: profile.JobType
        );

        var discoveredPostings = await provider.FetchPostingsAsync(criteria, cancellationToken);
        if (discoveredPostings.Count == 0)
            return;

        var matchedPostings = discoveredPostings
            .Where(p => _postingMatcher.IsMatch(p.Title, profile.TargetJobTitles, profile.Experience))
            .ToList();
        if (matchedPostings.Count == 0)
            return;

        var urls = matchedPostings
            .Select(p => p.Url.Trim())
            .ToList();

        var existingUrls = await _mediator.Send(
            new GetMatchingJobPostingsQuery(request.Source, urls),
            cancellationToken);

        var newPostings = matchedPostings
            .Where(p => !existingUrls.Contains(p.Url.Trim()))
            .ToList();

        if (newPostings.Count == 0)
            return;

        var items = newPostings
            .Select(p => new CreateJobPostingItem(
                Title: p.Title,
                Company: p.Company,
                Url: p.Url,
                Source: p.Source,
                Location: p.Location,
                PostedAt: p.PostedAt,
                DescriptionSnippet: p.DescriptionSnippet))
            .ToList();

        var createdItems = await _mediator.Send(new CreateJobPostingsCommand(items), cancellationToken);
        if (createdItems.Count == 0)
            return;

        var createdUrls = createdItems
            .Select(i => i.Url.Trim())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var insertedPostings = newPostings
            .Where(p => createdUrls.Contains(p.Url.Trim()))
            .DistinctBy(p => p.Url.Trim(), StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (insertedPostings.Count == 0)
            return;

        var candidateCompanies = insertedPostings
            .Select(p => p.Company)
            .Distinct()
            .ToList();

        var excludedCompaniesQuery = new GetMatchingExcludedCompaniesQuery(candidateCompanies);
        var excludedNames = await _mediator.Send(excludedCompaniesQuery, cancellationToken);

        var alertPostings = excludedNames.Count > 0
            ? insertedPostings
                .Where(p => !excludedNames.Contains(p.Company.Trim().ToLowerInvariant()))
                .ToList()
            : insertedPostings;

        foreach (var posting in alertPostings)
        {
            try
            {
                string message = _formatter.Format(posting);
                await _telegramService.SendMessageAsync(
                    profile.BotToken,
                    profile.TelegramChatId,
                    message,
                    "HTML",
                    cancellationToken
                );
            }
            catch
            {
                // Ignore individual delivery failure so remaining alerts can proceed
            }
        }
    }
}
