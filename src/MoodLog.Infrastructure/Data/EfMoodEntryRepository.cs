using Microsoft.EntityFrameworkCore;
using MoodLog.Core.Interfaces;
using MoodLog.Core.Models;

namespace MoodLog.Infrastructure.Data;

public sealed class EfMoodEntryRepository(MoodLogDbContext dbContext) : IMoodEntryRepository
{
    public async Task AddAsync(MoodEntry entry, CancellationToken cancellationToken = default)
    {
        await dbContext.MoodEntries.AddAsync(entry, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<MoodEntry>> GetRecentAsync(
        int count,
        CancellationToken cancellationToken = default) =>
        await dbContext.MoodEntries
            .AsNoTracking()
            .OrderByDescending(entry => entry.Date)
            .ThenByDescending(entry => entry.CreatedAt)
            .Take(count)
            .ToListAsync(cancellationToken);
}
