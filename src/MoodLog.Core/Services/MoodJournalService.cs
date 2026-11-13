using MoodLog.Core.Functional;
using MoodLog.Core.Interfaces;
using MoodLog.Core.Models;

namespace MoodLog.Core.Services;

public sealed class MoodJournalService(IMoodEntryRepository repository)
{
    public async Task<Result<MoodEntry, DomainError>> AddAsync(
        DateOnly date,
        MoodLevel level,
        IEnumerable<string>? tags,
        string? note,
        CancellationToken cancellationToken = default)
    {
        return await MoodEntry.Create(date, level, tags, note)
            .BindAsync(async entry =>
            {
                await repository.AddAsync(entry, cancellationToken);
                return Result<MoodEntry, DomainError>.Ok(entry);
            });
    }

    public Task<IReadOnlyList<MoodEntry>> GetRecentAsync(
        int count,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(count);
        return repository.GetRecentAsync(count, cancellationToken);
    }
}
