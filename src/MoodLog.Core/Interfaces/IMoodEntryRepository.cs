using MoodLog.Core.Models;

namespace MoodLog.Core.Interfaces;

public interface IMoodEntryRepository
{
    Task AddAsync(MoodEntry entry, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<MoodEntry>> GetRecentAsync(
        int count,
        CancellationToken cancellationToken = default);
}
