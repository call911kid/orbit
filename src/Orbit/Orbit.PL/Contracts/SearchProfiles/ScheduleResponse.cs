using Orbit.Common.Enums;

namespace Orbit.PL.Contracts.SearchProfiles;

public sealed record ScheduleResponse(
    PostingSource Source,
    int IntervalMinutes,
    bool IsEnabled
);
