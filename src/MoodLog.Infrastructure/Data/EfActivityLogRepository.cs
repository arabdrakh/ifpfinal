using Microsoft.EntityFrameworkCore;
using MoodLog.Core.Interfaces;
using MoodLog.Core.Models;

namespace MoodLog.Infrastructure.Data;

public sealed class EfActivityLogRepository(MoodLogDbContext dbContext) : IActivityLogRepository
{
    public async Task AddAsync(ActivityLog log, CancellationToken cancellationToken = default)
    {
        await dbContext.ActivityLogs.AddAsync(log, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ActivityLog>> GetForDateAsync(
        DateOnly date,
        CancellationToken cancellationToken = default) =>
        await dbContext.ActivityLogs
            .AsNoTracking()
            .Where(log => log.Date == date)
            .OrderByDescending(log => log.LoggedAt)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<ActivityLog>> GetForRangeAsync(
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken = default) =>
        await dbContext.ActivityLogs
            .AsNoTracking()
            .Where(log => log.Date >= startDate && log.Date <= endDate)
            .OrderBy(log => log.Date)
            .ToListAsync(cancellationToken);
}
