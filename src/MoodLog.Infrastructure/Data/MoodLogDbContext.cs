using Microsoft.EntityFrameworkCore;
using MoodLog.Core.Models;

namespace MoodLog.Infrastructure.Data;

public sealed class MoodLogDbContext(DbContextOptions<MoodLogDbContext> options) : DbContext(options)
{
    public DbSet<MoodEntry> MoodEntries => Set<MoodEntry>();
    public DbSet<WaterLog> WaterLogs => Set<WaterLog>();
    public DbSet<SleepLog> SleepLogs => Set<SleepLog>();
    public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();
    public DbSet<ProfileSettings> Profiles => Set<ProfileSettings>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MoodLogDbContext).Assembly);
    }
}
