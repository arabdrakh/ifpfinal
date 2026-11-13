namespace MoodLog.Core.Models;

public sealed record WellnessDay(
    DateOnly Date,
    double? AverageMood,
    long WaterConsumedMl,
    int WaterGoalMl,
    int? SleepMinutes,
    int ActivityMinutes,
    long Steps);
