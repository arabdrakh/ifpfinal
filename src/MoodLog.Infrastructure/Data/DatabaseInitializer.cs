using Microsoft.EntityFrameworkCore;

namespace MoodLog.Infrastructure.Data;

public static class DatabaseInitializer
{
    private const string InitialMigrationId = "20261113000000_InitialCreate";
    private const string SleepActivityMigrationId = "20261113000100_AddSleepAndActivity";
    private const string ProfileMigrationId = "20261113000200_AddProfileSettings";

    public static async Task InitializeAsync(
        MoodLogDbContext context,
        CancellationToken cancellationToken = default)
    {
        var connection = context.Database.GetDbConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT name FROM sqlite_master WHERE type = 'table'";
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        var tables = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        while (await reader.ReadAsync(cancellationToken))
        {
            tables.Add(reader.GetString(0));
        }

        await reader.DisposeAsync();
        if (tables.Contains("MoodEntries") && tables.Contains("WaterLogs"))
        {
            await BaselineLegacyDatabaseAsync(context, tables, cancellationToken);
        }

        await context.Database.MigrateAsync(cancellationToken);
    }

    private static async Task BaselineLegacyDatabaseAsync(
        MoodLogDbContext context,
        IReadOnlySet<string> tables,
        CancellationToken cancellationToken)
    {
        var connection = context.Database.GetDbConnection();
        await using var command = connection.CreateCommand();
        command.CommandText =
            """
            CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
                "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
                "ProductVersion" TEXT NOT NULL
            );
            """;
        await command.ExecuteNonQueryAsync(cancellationToken);

        var version = typeof(DbContext).Assembly.GetName().Version?.ToString(3) ?? "10.0.0";
        await RecordMigrationAsync(connection, InitialMigrationId, version, cancellationToken);
        if (tables.Contains("SleepLogs") && tables.Contains("ActivityLogs"))
        {
            await RecordMigrationAsync(connection, SleepActivityMigrationId, version, cancellationToken);
        }

        if (tables.Contains("ProfileSettings"))
        {
            await RecordMigrationAsync(connection, ProfileMigrationId, version, cancellationToken);
        }
    }

    private static async Task RecordMigrationAsync(
        System.Data.Common.DbConnection connection,
        string migrationId,
        string productVersion,
        CancellationToken cancellationToken)
    {
        await using var command = connection.CreateCommand();
        command.CommandText =
            """
            INSERT OR IGNORE INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
            VALUES (@migrationId, @productVersion);
            """;

        var migrationParameter = command.CreateParameter();
        migrationParameter.ParameterName = "@migrationId";
        migrationParameter.Value = migrationId;
        command.Parameters.Add(migrationParameter);

        var versionParameter = command.CreateParameter();
        versionParameter.ParameterName = "@productVersion";
        versionParameter.Value = productVersion;
        command.Parameters.Add(versionParameter);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}
