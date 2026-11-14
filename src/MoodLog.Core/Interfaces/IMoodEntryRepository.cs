using MoodLog.Core.Models;

namespace MoodLog.Core.Interfaces;

public interface IMoodEntryRepository
{
    Task<MoodEntry?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(MoodEntry entry, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task AddAsync(MoodEntry entry, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<MoodEntry>> GetRecentAsync(
        int count,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<MoodEntry>> GetForRangeAsync(
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken = default);
}
