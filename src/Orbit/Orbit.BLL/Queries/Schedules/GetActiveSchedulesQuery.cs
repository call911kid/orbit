using Orbit.Common.Enums;
using Orbit.DAL.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Orbit.BLL.Queries.Schedules;

public sealed record GetActiveSchedulesQuery : IRequest<List<ActiveScheduleResponse>>;

public sealed record ActiveScheduleResponse(
    Guid ProfileId,
    PostingSource Source,
    int IntervalMinutes
);

public sealed class GetActiveSchedulesQueryHandler : IRequestHandler<GetActiveSchedulesQuery, List<ActiveScheduleResponse>>
{
    private readonly OrbitDbContext _context;

    public GetActiveSchedulesQueryHandler(OrbitDbContext context)
    {
        _context = context;
    }

    public async Task<List<ActiveScheduleResponse>> Handle(GetActiveSchedulesQuery request, CancellationToken cancellationToken)
    {
        return await _context.SearchProfiles
            .AsNoTracking()
            .Where(sp => sp.IsActive && !sp.IsDeleted)
            .SelectMany(sp => sp.Schedules
                .Where(s => s.IsEnabled)
                .Select(s => new ActiveScheduleResponse(
                    sp.Id,
                    s.Source,
                    s.IntervalMinutes
                )))
            .ToListAsync(cancellationToken);
    }
}
