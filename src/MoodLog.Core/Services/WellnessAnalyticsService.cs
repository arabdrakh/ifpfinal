using MoodLog.Core.Interfaces;
using MoodLog.Core.Models;

namespace MoodLog.Core.Services;

public sealed class WellnessAnalyticsService(
    IMoodEntryRepository moodRepository,
    IWaterLogRepository waterRepository,
    ISleepLogRepository sleepRepository,
    IActivityLogRepository activityRepository)
{
    private sealed record WaterDay(long Consumed, int Goal);
    private sealed record ActivityDay(int Minutes, long Steps);

    public async Task<WellnessReport> GetReportAsync(
        DateOnly endDate,
        int days = 7,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(days, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(days, 90);

        var startDate = endDate.AddDays(-(days - 1));
        var insightsStartDate = endDate.AddDays(-13);
        var dataStartDate = startDate < insightsStartDate ? startDate : insightsStartDate;
        var moods = await moodRepository.GetForRangeAsync(dataStartDate, endDate, cancellationToken);
        var waterLogs = await waterRepository.GetForRangeAsync(dataStartDate, endDate, cancellationToken);
        var sleepLogs = await sleepRepository.GetForRangeAsync(dataStartDate, endDate, cancellationToken);
        var activityLogs = await activityRepository.GetForRangeAsync(dataStartDate, endDate, cancellationToken);

        var moodByDate = moods.GroupBy(entry => entry.Date)
            .ToDictionary(group => group.Key, group => group.Average(entry => (int)entry.Level));
        var waterByDate = waterLogs.GroupBy(log => log.Date)
            .ToDictionary(group => group.Key, group => new WaterDay(
                group.Sum(log => (long)log.AmountMl),
                group.OrderByDescending(log => log.LoggedAt).First().DailyGoalMl));
        var sleepByDate = sleepLogs.GroupBy(log => log.Date)
            .ToDictionary(group => group.Key, group => (int?)group.Average(log => log.DurationMinutes));
        var activityByDate = activityLogs.GroupBy(log => log.Date)
            .ToDictionary(group => group.Key, group => new ActivityDay(
                group.Sum(log => log.DurationMinutes),
                group.Sum(log => (long)log.Steps)));

        var reportDays = Enumerable.Range(0, days)
            .Select(offset => startDate.AddDays(offset))
            .Select(date =>
            {
                var water = waterByDate.GetValueOrDefault(date);
                var activity = activityByDate.GetValueOrDefault(date);
                return new WellnessDay(
                    date,
                    moodByDate.TryGetValue(date, out var mood) ? mood : null,
                    water?.Consumed ?? 0,
                    water?.Goal ?? 2000,
                    sleepByDate.GetValueOrDefault(date),
                    activity is not null,
                    activity?.Minutes ?? 0,
                    activity?.Steps ?? 0);
            })
            .ToArray();

        var insights = BuildInsights(
            startDate,
            endDate,
            days,
            moodByDate,
            waterByDate,
            sleepByDate,
            activityByDate);

        var moodValues = reportDays.Where(day => day.AverageMood.HasValue)
            .Select(day => day.AverageMood!.Value)
            .ToArray();
        var waterValues = reportDays.Where(day => day.WaterConsumedMl > 0)
            .Select(day => day.WaterConsumedMl)
            .ToArray();
        var sleepValues = reportDays.Where(day => day.SleepMinutes.HasValue)
            .Select(day => day.SleepMinutes!.Value)
            .ToArray();

        return new WellnessReport(
            startDate,
            endDate,
            reportDays,
            moodValues.Length == 0 ? null : moodValues.Average(),
            waterValues.Length == 0 ? null : (long)Math.Round(waterValues.Average()),
            sleepValues.Length == 0 ? null : (int)Math.Round(sleepValues.Average()),
            reportDays.Sum(day => day.ActivityMinutes),
            reportDays.Sum(day => day.Steps),
            insights);
    }

    private static IReadOnlyList<string> BuildInsights(
        DateOnly startDate,
        DateOnly endDate,
        int days,
        IReadOnlyDictionary<DateOnly, double> moodByDate,
        IReadOnlyDictionary<DateOnly, WaterDay> waterByDate,
        IReadOnlyDictionary<DateOnly, int?> sleepByDate,
        IReadOnlyDictionary<DateOnly, ActivityDay> activityByDate)
    {
        var insights = new List<string>();
        var streak = 0;
        for (var date = endDate; date >= endDate.AddDays(-6); date = date.AddDays(-1))
        {
            if (waterByDate.TryGetValue(date, out var water) && water.Consumed >= water.Goal)
            {
                streak++;
            }
            else
            {
                break;
            }
        }

        if (streak >= 5)
        {
            insights.Add($"You reached your water goal for {streak} days in a row.");
        }

        var currentWeekStart = endDate.AddDays(-6);
        var previousWeekStart = endDate.AddDays(-13);
        var currentSleep = Average(sleepByDate, currentWeekStart, endDate);
        var previousSleep = Average(sleepByDate, previousWeekStart, currentWeekStart.AddDays(-1));
        if (currentSleep.HasValue && previousSleep.HasValue && currentSleep.Value < previousSleep.Value * 0.9)
        {
            insights.Add("Your logged sleep duration was lower this week than the previous week.");
        }

        var currentActivity = Sum(activityByDate, currentWeekStart, endDate, item => item.Minutes);
        var previousActivity = Sum(activityByDate, previousWeekStart, currentWeekStart.AddDays(-1), item => item.Minutes);
        if (currentActivity > previousActivity && previousActivity > 0)
        {
            insights.Add("Your logged activity time increased compared with the previous week.");
        }

        var moodDays = moodByDate.Count(pair => pair.Key >= startDate && pair.Key <= endDate);
        if (moodDays >= Math.Min(4, days))
        {
            insights.Add($"You recorded your mood on {moodDays} days in this period.");
        }

        if (insights.Count == 0)
        {
            insights.Add("Keep logging your daily habits to discover patterns over time.");
        }

        return insights;
    }

    private static double? Average(
        IReadOnlyDictionary<DateOnly, int?> values,
        DateOnly startDate,
        DateOnly endDate)
    {
        var matching = values.Where(pair => pair.Key >= startDate && pair.Key <= endDate && pair.Value.HasValue)
            .Select(pair => (double)pair.Value!.Value)
            .ToArray();
        return matching.Length == 0 ? null : matching.Average();
    }

    private static long Sum<T>(
        IReadOnlyDictionary<DateOnly, T> values,
        DateOnly startDate,
        DateOnly endDate,
        Func<T, long> selector) =>
        values.Where(pair => pair.Key >= startDate && pair.Key <= endDate)
            .Sum(pair => selector(pair.Value));
}
