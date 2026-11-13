using MoodLog.Core.Models;

namespace MoodLog.Core.Interfaces;

public interface IWaterLogRepository
{
    Task AddAsync(WaterLog log, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<WaterLog>> GetForDateAsync(
        DateOnly date,
        CancellationToken cancellationToken = default);
}
