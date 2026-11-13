using Microsoft.EntityFrameworkCore;
using MoodLog.Core.Interfaces;
using MoodLog.Core.Models;

namespace MoodLog.Infrastructure.Data;

public sealed class EfProfileSettingsRepository(MoodLogDbContext dbContext) : IProfileSettingsRepository
{
    public Task<ProfileSettings?> GetAsync(CancellationToken cancellationToken = default) =>
        dbContext.Profiles.AsNoTracking().SingleOrDefaultAsync(
            settings => settings.Id == ProfileSettings.Default.Id,
            cancellationToken);

    public async Task SaveAsync(ProfileSettings settings, CancellationToken cancellationToken = default)
    {
        var existing = await dbContext.Profiles.FindAsync(
            [ProfileSettings.Default.Id],
            cancellationToken);
        if (existing is null)
        {
            await dbContext.Profiles.AddAsync(settings, cancellationToken);
        }
        else
        {
            dbContext.Entry(existing).CurrentValues.SetValues(settings);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
