using MoodLog.Core.Interfaces;
using MoodLog.Core.Models;
using MoodLog.Core.Services;

namespace MoodLog.Tests;

public class MoodJournalServiceTests
{
    [Fact]
    public async Task AddAsync_ShouldPersistValidMoodEntry()
    {
        var repository = new TestMoodEntryRepository();
        var service = new MoodJournalService(repository);

        var result = await service.AddAsync(
            new DateOnly(2026, 11, 13),
            MoodLevel.Good,
            ["work"],
            "A good day");

        Assert.True(result.IsSuccess);
        Assert.Equal(result.Value.Id, Assert.Single(repository.Entries).Id);
    }

    [Fact]
    public async Task AddAsync_ShouldNotPersistInvalidMoodEntry()
    {
        var repository = new TestMoodEntryRepository();
        var service = new MoodJournalService(repository);

        var result = await service.AddAsync(
            new DateOnly(2026, 11, 13),
            MoodLevel.Good,
            null,
            new string('a', 501));

        Assert.True(result.IsFailure);
        Assert.Empty(repository.Entries);
    }

    [Fact]
    public async Task GetRecentAsync_ShouldRejectNonPositiveCount()
    {
        var service = new MoodJournalService(new TestMoodEntryRepository());

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => service.GetRecentAsync(0));
    }

    private sealed class TestMoodEntryRepository : IMoodEntryRepository
    {
        public List<MoodEntry> Entries { get; } = [];

        public Task AddAsync(MoodEntry entry, CancellationToken cancellationToken = default)
        {
            Entries.Add(entry);
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<MoodEntry>> GetRecentAsync(
            int count,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<MoodEntry>>(Entries.Take(count).ToArray());

        public Task<IReadOnlyList<MoodEntry>> GetForRangeAsync(
            DateOnly startDate,
            DateOnly endDate,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<MoodEntry>>(
                Entries.Where(entry => entry.Date >= startDate && entry.Date <= endDate).ToArray());
    }
}
