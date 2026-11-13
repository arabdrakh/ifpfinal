namespace MoodLog.Core.Models;

public enum MoodLevel
{
    VeryBad = 1,
    Bad = 2,
    Neutral = 3,
    Good = 4,
    Excellent = 5
}

public static class MoodLevelExtensions
{
    public static string ToEmoji(this MoodLevel level) =>
        level switch
        {
            MoodLevel.VeryBad => "😫",
            MoodLevel.Bad => "🙁",
            MoodLevel.Neutral => "😐",
            MoodLevel.Good => "🙂",
            MoodLevel.Excellent => "😁",
            _ => "😐"
        };

    public static string ToLabel(this MoodLevel level) =>
        level switch
        {
            MoodLevel.VeryBad => "Very Bad",
            MoodLevel.Bad => "Bad",
            MoodLevel.Neutral => "Neutral",
            MoodLevel.Good => "Good",
            MoodLevel.Excellent => "Excellent",
            _ => "Neutral"
        };

    public static string ToColorHex(this MoodLevel level) =>
        level switch
        {
            MoodLevel.VeryBad => "#ef4444",
            MoodLevel.Bad => "#f97316",
            MoodLevel.Neutral => "#eab308",
            MoodLevel.Good => "#10b981",
            MoodLevel.Excellent => "#06b6d4",
            _ => "#94a3b8"
        };
}
