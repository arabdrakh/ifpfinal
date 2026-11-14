using MoodLog.Core.Models;

namespace MoodLog.Core.Interfaces;

public interface IActivityLogRepository
{
    Task<ActivityLog?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(ActivityLog entry, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task AddAsync(ActivityLog log, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ActivityLog>> GetForDateAsync(
        DateOnly date,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ActivityLog>> GetForRangeAsync(
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken = default);
}
