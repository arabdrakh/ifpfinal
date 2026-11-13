using MoodLog.Core.Models;

namespace MoodLog.Core.Interfaces;

public interface IActivityLogRepository
{
    Task AddAsync(ActivityLog log, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ActivityLog>> GetForDateAsync(
        DateOnly date,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ActivityLog>> GetForRangeAsync(
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken = default);
}
