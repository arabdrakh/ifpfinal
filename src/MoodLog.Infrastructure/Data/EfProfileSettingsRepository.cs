using Microsoft.EntityFrameworkCore;
using MoodLog.Core.Interfaces;
using MoodLog.Core.Models;

namespace MoodLog.Infrastructure.Data;

public sealed class EfProfileSettingsRepository(
    IDbContextFactory<MoodLogDbContext> contextFactory) : IProfileSettingsRepository
{
    public async Task<ProfileSettings?> GetAsync(CancellationToken cancellationToken = default)
    {
        await using var dbContext = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await dbContext.Profiles.AsNoTracking().SingleOrDefaultAsync(
            settings => settings.Id == ProfileSettings.Default.Id,
            cancellationToken);
    }

    public async Task SaveAsync(ProfileSettings settings, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await contextFactory.CreateDbContextAsync(cancellationToken);
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
