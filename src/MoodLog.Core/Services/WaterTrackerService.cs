using MoodLog.Core.Functional;
using MoodLog.Core.Interfaces;
using MoodLog.Core.Models;

namespace MoodLog.Core.Services;

public sealed class WaterTrackerService(IWaterLogRepository repository)
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
                return Result<WaterLog, DomainError>.Ok(log);
            });
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
        var dailyGoalMl = logs
            .OrderByDescending(log => log.LoggedAt)
            .Select(log => (int?)log.DailyGoalMl)
            .FirstOrDefault() ?? defaultGoalMl;
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
