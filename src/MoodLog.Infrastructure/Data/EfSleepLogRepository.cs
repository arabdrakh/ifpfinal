using Microsoft.EntityFrameworkCore;
using MoodLog.Core.Interfaces;
using MoodLog.Core.Models;

namespace MoodLog.Infrastructure.Data;

public sealed class EfSleepLogRepository(
    IDbContextFactory<MoodLogDbContext> contextFactory) : ISleepLogRepository
{
    public async Task<SleepLog?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await db.SleepLogs.AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
    }

    public async Task<bool> UpdateAsync(SleepLog log, CancellationToken cancellationToken = default)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        var existing = await db.SleepLogs.FindAsync([log.Id], cancellationToken);
        if (existing is null) return false;
        db.Entry(existing).CurrentValues.SetValues(log);
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        var existing = await db.SleepLogs.FindAsync([id], cancellationToken);
        if (existing is null) return false;
        db.SleepLogs.Remove(existing);
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task AddAsync(SleepLog log, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await contextFactory.CreateDbContextAsync(cancellationToken);
        await dbContext.SleepLogs.AddAsync(log, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SleepLog>> GetForDateAsync(
        DateOnly date,
        CancellationToken cancellationToken = default)
    {
        await using var dbContext = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await dbContext.SleepLogs
            .AsNoTracking()
            .Where(log => log.Date == date)
            .OrderByDescending(log => log.LoggedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SleepLog>> GetForRangeAsync(
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken = default)
    {
        await using var dbContext = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await dbContext.SleepLogs
            .AsNoTracking()
            .Where(log => log.Date >= startDate && log.Date <= endDate)
            .OrderBy(log => log.Date)
            .ToListAsync(cancellationToken);
    }
}
