using Microsoft.EntityFrameworkCore;
using MoodLog.Core.Interfaces;
using MoodLog.Core.Models;

namespace MoodLog.Infrastructure.Data;

public sealed class EfWaterLogRepository(MoodLogDbContext dbContext) : IWaterLogRepository
{
    public async Task AddAsync(WaterLog log, CancellationToken cancellationToken = default)
    {
        await dbContext.WaterLogs.AddAsync(log, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<WaterLog>> GetForDateAsync(
        DateOnly date,
        CancellationToken cancellationToken = default) =>
        await dbContext.WaterLogs
            .AsNoTracking()
            .Where(log => log.Date == date)
            .OrderByDescending(log => log.LoggedAt)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<WaterLog>> GetForRangeAsync(
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken = default) =>
        await dbContext.WaterLogs
            .AsNoTracking()
            .Where(log => log.Date >= startDate && log.Date <= endDate)
            .OrderBy(log => log.Date)
            .ThenBy(log => log.LoggedAt)
            .ToListAsync(cancellationToken);
}
