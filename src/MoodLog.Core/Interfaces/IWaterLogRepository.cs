using MoodLog.Core.Models;

namespace MoodLog.Core.Interfaces;

public interface IWaterLogRepository
{
    Task<WaterLog?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(WaterLog entry, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task AddAsync(WaterLog log, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<WaterLog>> GetForDateAsync(
        DateOnly date,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<WaterLog>> GetForRangeAsync(
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken = default);
}
