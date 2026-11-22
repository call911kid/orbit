using AutoMapper;
using Orbit.Common.Enums;
using Orbit.DAL.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Orbit.BLL.Queries.SearchProfiles;

public record GetAllSearchProfilesQuery : IRequest<List<GetAllSearchProfilesResponse>>;

public sealed record GetAllSearchProfilesResponse(
    Guid Id,
    string Name,
    string? Description,
    bool IsActive,
    List<string> TargetJobTitles,
    List<string> TargetLocations,
    int MaxPostingAgeMinutes,
    List<GetAllSearchProfilesScheduleResponse> Schedules,
    WorkplacePreference Workplace,
    ExperienceLevel Experience,
    JobType JobType,
    string TelegramChatId,
    string BotToken,
    DateTime CreatedAt
);

public sealed record GetAllSearchProfilesScheduleResponse(
    PostingSource Source,
    int IntervalMinutes,
    bool IsEnabled
);

public class GetAllSearchProfilesQueryHandler : IRequestHandler<GetAllSearchProfilesQuery, List<GetAllSearchProfilesResponse>>
{
    private readonly OrbitDbContext _context;
    private readonly IMapper _mapper;

    public GetAllSearchProfilesQueryHandler(OrbitDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<GetAllSearchProfilesResponse>> Handle(GetAllSearchProfilesQuery request, CancellationToken cancellationToken)
    {
        var entities = await _context.SearchProfiles
            .AsNoTracking()
            .Include(sp => sp.Schedules)
            .OrderByDescending(sp => sp.CreatedAt)
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<GetAllSearchProfilesResponse>>(entities);
    }
}
