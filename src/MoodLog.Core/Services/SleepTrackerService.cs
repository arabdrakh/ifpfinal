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
        return await SleepLog.Create(date, bedtime, wakeTime)
            .BindAsync(async log =>
            {
                await repository.AddAsync(log, cancellationToken);
                return Result<SleepLog, DomainError>.Ok(log);
            });
    }

    public Task<IReadOnlyList<SleepLog>> GetForDateAsync(
        DateOnly date,
        CancellationToken cancellationToken = default) =>
        repository.GetForDateAsync(date, cancellationToken);
}
