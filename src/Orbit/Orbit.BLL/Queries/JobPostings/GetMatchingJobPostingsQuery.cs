using MediatR;
using Microsoft.EntityFrameworkCore;
using Orbit.Common.Enums;
using Orbit.DAL.Context;

namespace Orbit.BLL.Queries.JobPostings;

public record GetMatchingJobPostingsQuery(
    PostingSource Source,
    IReadOnlyList<string> Urls
) : IRequest<HashSet<string>>;

public class GetMatchingJobPostingsQueryHandler
    : IRequestHandler<GetMatchingJobPostingsQuery, HashSet<string>>
{
    private readonly OrbitDbContext _context;

    public GetMatchingJobPostingsQueryHandler(OrbitDbContext context)
    {
        _context = context;
    }

    public async Task<HashSet<string>> Handle(
        GetMatchingJobPostingsQuery request,
        CancellationToken cancellationToken)
    {
        if (request.Urls.Count == 0)
            return new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        var normalizedUrls = request.Urls
            .Where(u => !string.IsNullOrWhiteSpace(u))
            .Select(u => u.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (normalizedUrls.Count == 0)
            return new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        var existingUrls = await _context.JobPostings
            .AsNoTracking()
            .IgnoreQueryFilters()
            .Where(jp => jp.Source == request.Source && normalizedUrls.Contains(jp.Url))
            .Select(jp => jp.Url)
            .ToListAsync(cancellationToken);

        return existingUrls.ToHashSet(StringComparer.OrdinalIgnoreCase);
    }
}
