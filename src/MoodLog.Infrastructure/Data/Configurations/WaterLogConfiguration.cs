using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MoodLog.Core.Models;

namespace MoodLog.Infrastructure.Data.Configurations;

public sealed class WaterLogConfiguration : IEntityTypeConfiguration<WaterLog>
{
    public void Configure(EntityTypeBuilder<WaterLog> builder)
    {
        builder.ToTable("WaterLogs");
        builder.HasKey(log => log.Id);
        builder.Property(log => log.Date).IsRequired();
        builder.Property(log => log.AmountMl).IsRequired();
        builder.Property(log => log.DailyGoalMl).IsRequired();
        builder.Property(log => log.LoggedAt).IsRequired();
        builder.HasIndex(log => log.Date);
    }
}
