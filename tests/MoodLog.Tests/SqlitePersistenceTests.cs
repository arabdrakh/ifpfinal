using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MoodLog.Core.Models;
using MoodLog.Infrastructure.Data;

namespace MoodLog.Tests;

public class SqlitePersistenceTests
{
    [Fact]
    public async Task SaveChanges_ShouldPersistMoodAndWaterLogs()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<MoodLogDbContext>()
            .UseSqlite(connection)
            .Options;

        var moodEntry = MoodEntry.Create(
            new DateOnly(2026, 11, 13),
            MoodLevel.Good,
            ["work", "rest"],
            "Felt balanced").Value;
        var waterLog = WaterLog.Create(
            new DateOnly(2026, 11, 13),
            300,
            2200).Value;

        await using (var context = new MoodLogDbContext(options))
        {
            await context.Database.EnsureCreatedAsync();
            context.MoodEntries.Add(moodEntry);
            context.WaterLogs.Add(waterLog);
            await context.SaveChangesAsync();
        }

        await using (var context = new MoodLogDbContext(options))
        {
            var savedMoodEntry = await context.MoodEntries.SingleAsync();
            var savedWaterLog = await context.WaterLogs.SingleAsync();

            Assert.Equal(moodEntry.Id, savedMoodEntry.Id);
            Assert.Equal(["work", "rest"], savedMoodEntry.Tags);
            Assert.Equal(moodEntry.Note, savedMoodEntry.Note);
            Assert.Equal(waterLog.Id, savedWaterLog.Id);
            Assert.Equal(300, savedWaterLog.AmountMl);
            Assert.Equal(2200, savedWaterLog.DailyGoalMl);
        }
    }
}
