using MoodLog.Core.Functional;
using MoodLog.Core.Interfaces;
using MoodLog.Core.Models;

namespace MoodLog.Core.Services;

public sealed class SleepTrackerService(ISleepLogRepository repository)
{
    public async Task<Result<SleepLog, DomainError>> AddAsync(
        DateOnly date,
        TimeOnly bedtime,
        TimeOnly wakeTime,
        CancellationToken cancellationToken = default)
    {
        var result = SleepLog.Create(date, bedtime, wakeTime);
        if (result.IsFailure)
        {
            return result;
        }

        await repository.AddAsync(result.Value, cancellationToken);
        return result;
    }

    public Task<IReadOnlyList<SleepLog>> GetForDateAsync(
        DateOnly date,
        CancellationToken cancellationToken = default) =>
        repository.GetForDateAsync(date, cancellationToken);
}
