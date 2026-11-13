using MoodLog.Core.Models;

namespace MoodLog.Core.Interfaces;

public interface IProfileSettingsRepository
{
    Task<ProfileSettings?> GetAsync(CancellationToken cancellationToken = default);

    Task SaveAsync(ProfileSettings settings, CancellationToken cancellationToken = default);
}
