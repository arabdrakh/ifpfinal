using MoodLog.Core.Functional;
using MoodLog.Core.Interfaces;
using MoodLog.Core.Models;

namespace MoodLog.Core.Services;

public sealed class MoodJournalService(IMoodEntryRepository repository, WellnessState? state = null)
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
                state?.NotifyChanged();
                return Result<MoodEntry, DomainError>.Ok(entry);
            });
    }

    public async Task<Result<MoodEntry, DomainError>> UpdateAsync(
        Guid id, DateOnly date, MoodLevel level, IEnumerable<string>? tags, string? note, CancellationToken cancellationToken = default)
    {
        var existing = await repository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
            return Result<MoodEntry, DomainError>.Fail(DomainError.NotFound("Entry not found."));

        var validated = MoodEntry.Create(date, level, tags, note);
        if (validated.IsFailure)
            return validated;

        var updated = validated.Value with { Id = existing.Id, CreatedAt = existing.CreatedAt };
        if (!await repository.UpdateAsync(updated, cancellationToken))
            return Result<MoodEntry, DomainError>.Fail(DomainError.NotFound("Entry not found."));

        state?.NotifyChanged();
        return Result<MoodEntry, DomainError>.Ok(updated);
    }

    public async Task<Result<Unit, DomainError>> DeleteAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        if (!await repository.DeleteAsync(id, cancellationToken))
            return Result<Unit, DomainError>.Fail(DomainError.NotFound("Entry not found."));

        state?.NotifyChanged();
        return Result<Unit, DomainError>.Ok(Unit.Value);
    }

    public Task<IReadOnlyList<MoodEntry>> GetRecentAsync(
        int count,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(count);
        return repository.GetRecentAsync(count, cancellationToken);
    }
}
