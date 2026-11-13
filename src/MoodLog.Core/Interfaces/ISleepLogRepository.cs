using MoodLog.Core.Models;

namespace MoodLog.Core.Interfaces;

public interface ISleepLogRepository
{
    Task AddAsync(SleepLog log, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SleepLog>> GetForDateAsync(
        DateOnly date,
        CancellationToken cancellationToken = default);
}
