using MoodLog.Core.Models;

namespace MoodLog.Core.Interfaces;

public interface ISleepLogRepository
{
    Task<SleepLog?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(SleepLog entry, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task AddAsync(SleepLog log, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SleepLog>> GetForDateAsync(
        DateOnly date,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SleepLog>> GetForRangeAsync(
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken = default);
}
