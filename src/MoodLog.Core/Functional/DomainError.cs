namespace MoodLog.Core.Functional;

public record DomainError(string Code, string Message)
{
    public static DomainError Validation(string message) => new("VALIDATION_ERROR", message);
    public static DomainError NotFound(string message) => new("NOT_FOUND", message);
    public static DomainError Conflict(string message) => new("CONFLICT", message);
}
