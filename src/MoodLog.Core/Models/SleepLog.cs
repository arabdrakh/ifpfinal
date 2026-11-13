using MoodLog.Core.Functional;

namespace MoodLog.Core.Models;

public sealed record SleepLog(
    Guid Id,
    DateOnly Date,
    TimeOnly Bedtime,
    TimeOnly WakeTime,
    int DurationMinutes,
    DateTime LoggedAt)
{
    public static Result<SleepLog, DomainError> Create(
        DateOnly date,
        TimeOnly bedtime,
        TimeOnly wakeTime)
    {
        var durationMinutes = (int)(wakeTime - bedtime).TotalMinutes;
        if (durationMinutes <= 0)
        {
            durationMinutes += 24 * 60;
        }

        if (durationMinutes > 16 * 60)
        {
            return Result<SleepLog, DomainError>.Fail(
                DomainError.Validation("Sleep duration cannot exceed 16 hours."));
        }

        return Result<SleepLog, DomainError>.Ok(new SleepLog(
            Guid.NewGuid(),
            date,
            bedtime,
            wakeTime,
            durationMinutes,
            DateTime.UtcNow));
    }
}
