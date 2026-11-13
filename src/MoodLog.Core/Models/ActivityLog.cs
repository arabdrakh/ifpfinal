using MoodLog.Core.Functional;

namespace MoodLog.Core.Models;

public sealed record ActivityLog(
    Guid Id,
    DateOnly Date,
    ActivityType Type,
    int DurationMinutes,
    int Steps,
    DateTime LoggedAt)
{
    public static Result<ActivityLog, DomainError> Create(
        DateOnly date,
        ActivityType type,
        int durationMinutes,
        int steps)
    {
        if (!Enum.IsDefined(type))
        {
            return Result<ActivityLog, DomainError>.Fail(
                DomainError.Validation("Activity type is invalid."));
        }

        if (durationMinutes is < 1 or > 1440)
        {
            return Result<ActivityLog, DomainError>.Fail(
                DomainError.Validation("Duration must be between 1 and 1440 minutes."));
        }

        if (steps is < 0 or > 100000)
        {
            return Result<ActivityLog, DomainError>.Fail(
                DomainError.Validation("Steps must be between 0 and 100000."));
        }

        return Result<ActivityLog, DomainError>.Ok(new ActivityLog(
            Guid.NewGuid(),
            date,
            type,
            durationMinutes,
            steps,
            DateTime.UtcNow));
    }
}
