using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MoodLog.Core.Models;

namespace MoodLog.Infrastructure.Data.Configurations;

public sealed class ActivityLogConfiguration : IEntityTypeConfiguration<ActivityLog>
{
    public void Configure(EntityTypeBuilder<ActivityLog> builder)
    {
        builder.ToTable("ActivityLogs");
        builder.HasKey(log => log.Id);
        builder.Property(log => log.Date).IsRequired();
        builder.Property(log => log.Type).HasConversion<int>().IsRequired();
        builder.Property(log => log.DurationMinutes).IsRequired();
        builder.Property(log => log.Steps).IsRequired();
        builder.Property(log => log.LoggedAt).IsRequired();
        builder.HasIndex(log => log.Date);
    }
}
