using Microsoft.EntityFrameworkCore;
using MoodLog.Core.Interfaces;
using MoodLog.Core.Models;

namespace MoodLog.Infrastructure.Data;

public sealed class EfActivityLogRepository(
    IDbContextFactory<MoodLogDbContext> contextFactory) : IActivityLogRepository
{
    public async Task<ActivityLog?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await db.ActivityLogs.AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
    }

    public async Task<bool> UpdateAsync(ActivityLog log, CancellationToken cancellationToken = default)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        var existing = await db.ActivityLogs.FindAsync([log.Id], cancellationToken);
        if (existing is null) return false;
        db.Entry(existing).CurrentValues.SetValues(log);
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        var existing = await db.ActivityLogs.FindAsync([id], cancellationToken);
        if (existing is null) return false;
        db.ActivityLogs.Remove(existing);
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task AddAsync(ActivityLog log, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await contextFactory.CreateDbContextAsync(cancellationToken);
        await dbContext.ActivityLogs.AddAsync(log, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ActivityLog>> GetForDateAsync(
        DateOnly date,
        CancellationToken cancellationToken = default)
    {
        await using var dbContext = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await dbContext.ActivityLogs
            .AsNoTracking()
            .Where(log => log.Date == date)
            .OrderByDescending(log => log.LoggedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ActivityLog>> GetForRangeAsync(
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken = default)
    {
        await using var dbContext = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await dbContext.ActivityLogs
            .AsNoTracking()
            .Where(log => log.Date >= startDate && log.Date <= endDate)
            .OrderBy(log => log.Date)
            .ToListAsync(cancellationToken);
    }
}
