using MoodLog.Core.Functional;
using MoodLog.Core.Interfaces;
using MoodLog.Core.Models;

namespace MoodLog.Core.Services;

public sealed class ProfileSettingsService(IProfileSettingsRepository repository, WellnessState? state = null)
{
    public async Task<ProfileSettings> GetAsync(CancellationToken cancellationToken = default)
    {
        var stored = await repository.GetAsync(cancellationToken);
        return Option<ProfileSettings>.From(stored).ValueOr(ProfileSettings.Default);
    }

    public async Task<Result<ProfileSettings, DomainError>> SaveAsync(
        string? displayName,
        int dailyWaterGoalMl,
        int dailySleepGoalMinutes,
        int dailyStepGoal,
        CancellationToken cancellationToken = default)
    {
        return await ProfileSettings.Create(
                displayName,
                dailyWaterGoalMl,
                dailySleepGoalMinutes,
                dailyStepGoal)
            .BindAsync(async settings =>
            {
                await repository.SaveAsync(settings, cancellationToken);
                state?.NotifyChanged();
                return Result<ProfileSettings, DomainError>.Ok(settings);
            });
    }
}
