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

    public async Task<IReadOnlyList<MoodEntry>> GetForRangeAsync(
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken = default) =>
        await dbContext.MoodEntries
            .AsNoTracking()
            .Where(entry => entry.Date >= startDate && entry.Date <= endDate)
            .OrderBy(entry => entry.Date)
            .ThenBy(entry => entry.CreatedAt)
            .ToListAsync(cancellationToken);
}
