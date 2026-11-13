using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MoodLog.Core.Models;

namespace MoodLog.Infrastructure.Data.Configurations;

public sealed class SleepLogConfiguration : IEntityTypeConfiguration<SleepLog>
{
    public void Configure(EntityTypeBuilder<SleepLog> builder)
    {
        builder.ToTable("SleepLogs");
        builder.HasKey(log => log.Id);
        builder.Property(log => log.Date).IsRequired();
        builder.Property(log => log.Bedtime).IsRequired();
        builder.Property(log => log.WakeTime).IsRequired();
        builder.Property(log => log.DurationMinutes).IsRequired();
        builder.Property(log => log.LoggedAt).IsRequired();
        builder.HasIndex(log => log.Date);
    }
}
