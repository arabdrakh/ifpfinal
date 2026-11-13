using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MoodLog.Core.Models;

namespace MoodLog.Infrastructure.Data.Configurations;

public sealed class ProfileSettingsConfiguration : IEntityTypeConfiguration<ProfileSettings>
{
    public void Configure(EntityTypeBuilder<ProfileSettings> builder)
    {
        builder.ToTable("ProfileSettings");
        builder.HasKey(settings => settings.Id);
        builder.Property(settings => settings.DisplayName).HasMaxLength(40).IsRequired();
        builder.Property(settings => settings.DailyWaterGoalMl).IsRequired();
        builder.Property(settings => settings.DailySleepGoalMinutes).IsRequired();
        builder.Property(settings => settings.DailyStepGoal).IsRequired();
    }
}
