namespace MoodLog.Core.Models;

public sealed record WellnessReport(
    DateOnly StartDate,
    DateOnly EndDate,
    IReadOnlyList<WellnessDay> Days,
    double? AverageMood,
    long? AverageWaterMl,
    int? AverageSleepMinutes,
    int TotalActivityMinutes,
    long TotalSteps,
    IReadOnlyList<string> Insights);
