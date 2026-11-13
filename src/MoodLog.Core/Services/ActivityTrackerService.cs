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
        var result = ActivityLog.Create(date, type, durationMinutes, steps);
        if (result.IsFailure)
        {
            return result;
        }

        await repository.AddAsync(result.Value, cancellationToken);
        return result;
    }

    public Task<IReadOnlyList<ActivityLog>> GetForDateAsync(
        DateOnly date,
        CancellationToken cancellationToken = default) =>
        repository.GetForDateAsync(date, cancellationToken);
}
