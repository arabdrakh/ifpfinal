using MoodLog.Core.Functional;
using MoodLog.Core.Interfaces;
using MoodLog.Core.Models;

namespace MoodLog.Core.Services;

public sealed class ProfileSettingsService(IProfileSettingsRepository repository)
{
    public async Task<ProfileSettings> GetAsync(CancellationToken cancellationToken = default) =>
        await repository.GetAsync(cancellationToken) ?? ProfileSettings.Default;

    public async Task<Result<ProfileSettings, DomainError>> SaveAsync(
        string? displayName,
        int dailyWaterGoalMl,
        int dailySleepGoalMinutes,
        int dailyStepGoal,
        CancellationToken cancellationToken = default)
    {
        var result = ProfileSettings.Create(
            displayName,
            dailyWaterGoalMl,
            dailySleepGoalMinutes,
            dailyStepGoal);
        if (result.IsFailure)
        {
            return result;
        }

        await repository.SaveAsync(result.Value, cancellationToken);
        return result;
    }
}
