using MoodLog.Core.Functional;

namespace MoodLog.Core.Models;

public record WaterLog(
    Guid Id,
    DateOnly Date,
    int AmountMl,
    int DailyGoalMl,
    DateTime LoggedAt)
{
    public static Result<WaterLog, DomainError> Create(
        DateOnly date,
        int amountMl,
        int dailyGoalMl = 2000)
    {
        if (amountMl <= 0 || amountMl > 5000)
        {
            return Result<WaterLog, DomainError>.Fail(
                DomainError.Validation("Amount must be between 1 and 5000 ml."));
        }

        if (dailyGoalMl < 500 || dailyGoalMl > 10000)
        {
            return Result<WaterLog, DomainError>.Fail(
                DomainError.Validation("Daily goal must be between 500 and 10000 ml."));
        }

        var log = new WaterLog(
            Guid.NewGuid(),
            date,
            amountMl,
            dailyGoalMl,
            DateTime.UtcNow);

        return Result<WaterLog, DomainError>.Ok(log);
    }
}
