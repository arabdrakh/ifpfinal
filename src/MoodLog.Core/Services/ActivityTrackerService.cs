using MoodLog.Core.Functional;
using MoodLog.Core.Interfaces;
using MoodLog.Core.Models;

namespace MoodLog.Core.Services;

public sealed class ActivityTrackerService(IActivityLogRepository repository)
{
    public async Task<Result<ActivityLog, DomainError>> AddAsync(
        DateOnly date,
        ActivityType type,
        int durationMinutes,
        int steps,
        CancellationToken cancellationToken = default)
    {
        return await ActivityLog.Create(date, type, durationMinutes, steps)
            .BindAsync(async log =>
            {
                await repository.AddAsync(log, cancellationToken);
                return Result<ActivityLog, DomainError>.Ok(log);
            });
    }

    public Task<IReadOnlyList<ActivityLog>> GetForDateAsync(
        DateOnly date,
        CancellationToken cancellationToken = default) =>
        repository.GetForDateAsync(date, cancellationToken);
}
