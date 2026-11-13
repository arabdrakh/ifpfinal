using MoodLog.Core.Functional;

namespace MoodLog.Core.Models;

public sealed record ProfileSettings(
    int Id,
    string DisplayName,
    int DailyWaterGoalMl,
    int DailySleepGoalMinutes,
    int DailyStepGoal)
{
    public static ProfileSettings Default { get; } = new(1, "Friend", 2000, 480, 8000);

    public static Result<ProfileSettings, DomainError> Create(
        string? displayName,
        int dailyWaterGoalMl,
        int dailySleepGoalMinutes,
        int dailyStepGoal)
    {
        var cleanedName = (displayName ?? string.Empty).Trim();
        if (cleanedName.Length is < 1 or > 40)
        {
            return Result<ProfileSettings, DomainError>.Fail(
                DomainError.Validation("Name must contain between 1 and 40 characters."));
        }

        if (dailyWaterGoalMl is < 500 or > 10000)
        {
            return Result<ProfileSettings, DomainError>.Fail(
                DomainError.Validation("Daily water goal must be between 500 and 10000 ml."));
        }

        if (dailySleepGoalMinutes is < 240 or > 960)
        {
            return Result<ProfileSettings, DomainError>.Fail(
                DomainError.Validation("Daily sleep goal must be between 4 and 16 hours."));
        }

        if (dailyStepGoal is < 1000 or > 50000)
        {
            return Result<ProfileSettings, DomainError>.Fail(
                DomainError.Validation("Daily step goal must be between 1000 and 50000."));
        }

        return Result<ProfileSettings, DomainError>.Ok(new ProfileSettings(
            1,
            cleanedName,
            dailyWaterGoalMl,
            dailySleepGoalMinutes,
            dailyStepGoal));
    }
}
