using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MoodLog.Core.Models;

namespace MoodLog.Infrastructure.Data.Configurations;

public sealed class MoodEntryConfiguration : IEntityTypeConfiguration<MoodEntry>
{
    public void Configure(EntityTypeBuilder<MoodEntry> builder)
    {
        builder.ToTable("MoodEntries");
        builder.HasKey(entry => entry.Id);
        builder.Property(entry => entry.Date).IsRequired();
        builder.Property(entry => entry.Level).HasConversion<int>().IsRequired();
        builder.Property(entry => entry.Tags)
            .HasConversion(TagsConverter)
            .Metadata.SetValueComparer(TagsComparer);
        builder.Property(entry => entry.Note).HasMaxLength(500).IsRequired();
        builder.Property(entry => entry.CreatedAt).IsRequired();
        builder.HasIndex(entry => entry.Date);
    }

    private static ValueConverter<IReadOnlyList<string>, string> TagsConverter =>
        new(
            tags => JsonSerializer.Serialize(tags, (JsonSerializerOptions?)null),
            json => JsonSerializer.Deserialize<List<string>>(json, (JsonSerializerOptions?)null)!);

    private static ValueComparer<IReadOnlyList<string>> TagsComparer =>
        new(
            (left, right) => left != null && right != null && left.SequenceEqual(right),
            tags => tags == null
                ? 0
                : tags.Aggregate(0, (hash, tag) => HashCode.Combine(hash, tag.GetHashCode())),
            tags => tags == null ? Array.Empty<string>() : tags.ToArray());
}
