using Microsoft.EntityFrameworkCore;
using MoodLog.Core.Interfaces;
using MoodLog.Core.Models;

namespace MoodLog.Infrastructure.Data;

public sealed class EfMoodEntryRepository(
    IDbContextFactory<MoodLogDbContext> contextFactory) : IMoodEntryRepository
{
    public async Task<MoodEntry?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await db.MoodEntries.AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
    }

    public async Task<bool> UpdateAsync(MoodEntry entry, CancellationToken cancellationToken = default)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        var existing = await db.MoodEntries.FindAsync([entry.Id], cancellationToken);
        if (existing is null) return false;
        db.Entry(existing).CurrentValues.SetValues(entry);
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        var existing = await db.MoodEntries.FindAsync([id], cancellationToken);
        if (existing is null) return false;
        db.MoodEntries.Remove(existing);
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task AddAsync(MoodEntry entry, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await contextFactory.CreateDbContextAsync(cancellationToken);
        await dbContext.MoodEntries.AddAsync(entry, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<MoodEntry>> GetRecentAsync(
        int count,
        CancellationToken cancellationToken = default)
    {
        await using var dbContext = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await dbContext.MoodEntries
            .AsNoTracking()
            .OrderByDescending(entry => entry.Date)
            .ThenByDescending(entry => entry.CreatedAt)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<MoodEntry>> GetForRangeAsync(
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken = default)
    {
        await using var dbContext = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await dbContext.MoodEntries
            .AsNoTracking()
            .Where(entry => entry.Date >= startDate && entry.Date <= endDate)
            .OrderBy(entry => entry.Date)
            .ThenBy(entry => entry.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
