namespace MoodLog.Core.Models;

public sealed record WaterProgress(
    long ConsumedMl,
    int DailyGoalMl,
    long RemainingMl,
    int ProgressPercent,
    bool GoalReached);
