using MoodLog.Core.Functional;
using MoodLog.Core.Interfaces;
using MoodLog.Core.Models;

namespace MoodLog.Core.Services;

public sealed class WaterTrackerService(IWaterLogRepository repository, WellnessState? state = null)
{
    public async Task<Result<WaterLog, DomainError>> AddAsync(
        DateOnly date,
        int amountMl,
        int dailyGoalMl = 2000,
        CancellationToken cancellationToken = default)
    {
        return await WaterLog.Create(date, amountMl, dailyGoalMl)
            .BindAsync(async log =>
            {
                await repository.AddAsync(log, cancellationToken);
                state?.NotifyChanged();
                return Result<WaterLog, DomainError>.Ok(log);
            });
    }

    public async Task<Result<WaterLog, DomainError>> UpdateAsync(
        Guid id, DateOnly date, int amountMl, int dailyGoalMl = 2000, CancellationToken cancellationToken = default)
    {
        var existing = await repository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
            return Result<WaterLog, DomainError>.Fail(DomainError.NotFound("Entry not found."));

        var validated = WaterLog.Create(date, amountMl, dailyGoalMl);
        if (validated.IsFailure)
            return validated;

        var updated = validated.Value with { Id = existing.Id, LoggedAt = existing.LoggedAt };
        if (!await repository.UpdateAsync(updated, cancellationToken))
            return Result<WaterLog, DomainError>.Fail(DomainError.NotFound("Entry not found."));

        state?.NotifyChanged();
        return Result<WaterLog, DomainError>.Ok(updated);
    }

    public async Task<Result<Unit, DomainError>> DeleteAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        if (!await repository.DeleteAsync(id, cancellationToken))
            return Result<Unit, DomainError>.Fail(DomainError.NotFound("Entry not found."));

        state?.NotifyChanged();
        return Result<Unit, DomainError>.Ok(Unit.Value);
    }

    public async Task<WaterProgress> GetDailyProgressAsync(
        DateOnly date,
        int defaultGoalMl = 2000,
        CancellationToken cancellationToken = default)
    {
        if (defaultGoalMl is < 500 or > 10000)
        {
            throw new ArgumentOutOfRangeException(nameof(defaultGoalMl));
        }

        var logs = await repository.GetForDateAsync(date, cancellationToken);
        var consumedMl = logs.Sum(log => (long)log.AmountMl);
        var dailyGoalMl = defaultGoalMl;
        var remainingMl = Math.Max(0, dailyGoalMl - consumedMl);
        var progressPercent = (int)Math.Min(100, consumedMl * 100d / dailyGoalMl);

        return new WaterProgress(
            consumedMl,
            dailyGoalMl,
            remainingMl,
            progressPercent,
            consumedMl >= dailyGoalMl);
    }

    public Task<IReadOnlyList<WaterLog>> GetLogsForDateAsync(
        DateOnly date,
        CancellationToken cancellationToken = default) =>
        repository.GetForDateAsync(date, cancellationToken);
}
