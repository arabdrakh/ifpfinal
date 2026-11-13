using MoodLog.Core.Functional;

namespace MoodLog.Core.Models;

public record MoodEntry(
    Guid Id,
    DateOnly Date,
    MoodLevel Level,
    IReadOnlyList<string> Tags,
    string Note,
    DateTime CreatedAt)
{
    public static Result<MoodEntry, DomainError> Create(
        DateOnly date,
        MoodLevel level,
        IEnumerable<string>? tags,
        string? note)
    {
        if (!Enum.IsDefined(level))
        {
            return Result<MoodEntry, DomainError>.Fail(
                DomainError.Validation("Mood level is invalid."));
        }

        var cleanedNote = (note ?? string.Empty).Trim();
        if (cleanedNote.Length > 500)
        {
            return Result<MoodEntry, DomainError>.Fail(
                DomainError.Validation("Note cannot exceed 500 characters."));
        }

        var tagList = (tags ?? Enumerable.Empty<string>())
            .Select(t => t.Trim())
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Take(10)
            .ToList();

        var entry = new MoodEntry(
            Guid.NewGuid(),
            date,
            level,
            tagList,
            cleanedNote,
            DateTime.UtcNow);

        return Result<MoodEntry, DomainError>.Ok(entry);
    }
}
