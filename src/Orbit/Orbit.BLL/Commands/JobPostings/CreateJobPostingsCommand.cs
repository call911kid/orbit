using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Orbit.Common.Enums;
using Orbit.DAL.Context;
using Orbit.DAL.Extensions;
using Orbit.DAL.Models;

namespace Orbit.BLL.Commands.JobPostings;

public sealed record CreateJobPostingItem(
    string Title,
    string Company,
    string Url,
    PostingSource Source,
    string? Location = null,
    DateTime? PostedAt = null,
    string? DescriptionSnippet = null
);

public record CreateJobPostingsCommand(
    IReadOnlyList<CreateJobPostingItem> Postings
) : IRequest<IReadOnlyList<CreateJobPostingItem>>;

public class CreateJobPostingsCommandHandler : IRequestHandler<CreateJobPostingsCommand, IReadOnlyList<CreateJobPostingItem>>
{
    private readonly OrbitDbContext _context;
    private readonly IMapper _mapper;

    public CreateJobPostingsCommandHandler(OrbitDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<CreateJobPostingItem>> Handle(
        CreateJobPostingsCommand request,
        CancellationToken cancellationToken)
    {
        if (request.Postings is not { Count: > 0 })
            return [];

        var pending = _mapper.Map<List<JobPosting>>(request.Postings)
            .DistinctBy(p => ToKey(p.Source, p.Url))
            .ToList();

        var inserted = new List<JobPosting>();

        while (pending.Count > 0)
        {
            try
            {
                _context.JobPostings.AddRange(pending);
                await _context.SaveChangesAsync(cancellationToken);
                inserted.AddRange(pending);
                break;
            }
            catch (DbUpdateException ex) when (ex.IsUniqueConstraintViolation())
            {
                _context.ChangeTracker.Clear();

                var sources = pending.Select(p => p.Source).Distinct().ToList();
                var urls = pending.Select(p => p.Url).Distinct(StringComparer.OrdinalIgnoreCase).ToList();

                var committed = await _context.JobPostings
                    .AsNoTracking()
                    .IgnoreQueryFilters()
                    .Where(jp => sources.Contains(jp.Source) && urls.Contains(jp.Url))
                    .Select(jp => new { jp.Source, jp.Url })
                    .ToListAsync(cancellationToken);

                var committedKeys = committed
                    .Select(c => ToKey(c.Source, c.Url))
                    .ToHashSet();

                var nextPending = pending
                    .Where(p => !committedKeys.Contains(ToKey(p.Source, p.Url)))
                    .ToList();

                if (nextPending.Count == pending.Count)
                    throw;

                pending = nextPending;
            }
        }

        if (inserted.Count == 0)
            return [];

        var insertedKeys = inserted
            .Select(i => ToKey(i.Source, i.Url))
            .ToHashSet();

        return request.Postings
            .Where(p => insertedKeys.Contains(ToKey(p.Source, p.Url)))
            .DistinctBy(p => ToKey(p.Source, p.Url))
            .ToList();
    }

    private static (PostingSource Source, string Url) ToKey(PostingSource source, string url) =>
        (source, url.Trim().ToLowerInvariant());
}
