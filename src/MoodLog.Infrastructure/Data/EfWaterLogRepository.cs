using Microsoft.EntityFrameworkCore;
using MoodLog.Core.Interfaces;
using MoodLog.Core.Models;

namespace MoodLog.Infrastructure.Data;

public sealed class EfWaterLogRepository(
    IDbContextFactory<MoodLogDbContext> contextFactory) : IWaterLogRepository
{
    public async Task<WaterLog?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await db.WaterLogs.AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
    }

    public async Task<bool> UpdateAsync(WaterLog log, CancellationToken cancellationToken = default)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        var existing = await db.WaterLogs.FindAsync([log.Id], cancellationToken);
        if (existing is null) return false;
        db.Entry(existing).CurrentValues.SetValues(log);
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        var existing = await db.WaterLogs.FindAsync([id], cancellationToken);
        if (existing is null) return false;
        db.WaterLogs.Remove(existing);
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task AddAsync(WaterLog log, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await contextFactory.CreateDbContextAsync(cancellationToken);
        await dbContext.WaterLogs.AddAsync(log, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<WaterLog>> GetForDateAsync(
        DateOnly date,
        CancellationToken cancellationToken = default)
    {
        await using var dbContext = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await dbContext.WaterLogs
            .AsNoTracking()
            .Where(log => log.Date == date)
            .OrderByDescending(log => log.LoggedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<WaterLog>> GetForRangeAsync(
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken = default)
    {
        await using var dbContext = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await dbContext.WaterLogs
            .AsNoTracking()
            .Where(log => log.Date >= startDate && log.Date <= endDate)
            .OrderBy(log => log.Date)
            .ThenBy(log => log.LoggedAt)
            .ToListAsync(cancellationToken);
    }
}
