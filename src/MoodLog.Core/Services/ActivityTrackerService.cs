using MoodLog.Core.Functional;
using MoodLog.Core.Interfaces;
using MoodLog.Core.Models;

namespace MoodLog.Core.Services;

public sealed class ActivityTrackerService(IActivityLogRepository repository, WellnessState? state = null)
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
                state?.NotifyChanged();
                return Result<ActivityLog, DomainError>.Ok(log);
            });
    }

    public async Task<Result<ActivityLog, DomainError>> UpdateAsync(
        Guid id, DateOnly date, ActivityType type, int durationMinutes, int steps, CancellationToken cancellationToken = default)
    {
        var existing = await repository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
            return Result<ActivityLog, DomainError>.Fail(DomainError.NotFound("Entry not found."));

        var validated = ActivityLog.Create(date, type, durationMinutes, steps);
        if (validated.IsFailure)
            return validated;

        var updated = validated.Value with { Id = existing.Id, LoggedAt = existing.LoggedAt };
        if (!await repository.UpdateAsync(updated, cancellationToken))
            return Result<ActivityLog, DomainError>.Fail(DomainError.NotFound("Entry not found."));

        state?.NotifyChanged();
        return Result<ActivityLog, DomainError>.Ok(updated);
    }

    public async Task<Result<Unit, DomainError>> DeleteAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        if (!await repository.DeleteAsync(id, cancellationToken))
            return Result<Unit, DomainError>.Fail(DomainError.NotFound("Entry not found."));

        state?.NotifyChanged();
        return Result<Unit, DomainError>.Ok(Unit.Value);
    }

    public Task<IReadOnlyList<ActivityLog>> GetForDateAsync(
        DateOnly date,
        CancellationToken cancellationToken = default) =>
        repository.GetForDateAsync(date, cancellationToken);
}
