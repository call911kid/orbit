using Orbit.Common.Enums;

namespace Orbit.BLL.Interfaces;

public interface IPostingScheduleManager
{
    void ScheduleRecurringIngestion(Guid profileId, PostingSource source, int intervalMinutes);
    void RemoveRecurringIngestion(Guid profileId, PostingSource source);
    Task SyncSchedulesAsync(CancellationToken cancellationToken = default);
}
