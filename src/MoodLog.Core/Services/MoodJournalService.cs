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
        var result = MoodEntry.Create(date, level, tags, note);
        if (result.IsFailure)
        {
            return result;
        }

        await repository.AddAsync(result.Value, cancellationToken);
        return result;
    }

    public Task<IReadOnlyList<MoodEntry>> GetRecentAsync(
        int count,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(count);
        return repository.GetRecentAsync(count, cancellationToken);
    }
}
