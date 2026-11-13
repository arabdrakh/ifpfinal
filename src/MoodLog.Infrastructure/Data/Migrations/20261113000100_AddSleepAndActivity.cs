using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using MoodLog.Infrastructure.Data;

namespace MoodLog.Infrastructure.Data.Migrations;

[DbContext(typeof(MoodLogDbContext))]
[Migration("20261113000100_AddSleepAndActivity")]
public sealed class AddSleepAndActivity : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "SleepLogs",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                Date = table.Column<DateOnly>(type: "TEXT", nullable: false),
                Bedtime = table.Column<TimeOnly>(type: "TEXT", nullable: false),
                WakeTime = table.Column<TimeOnly>(type: "TEXT", nullable: false),
                DurationMinutes = table.Column<int>(type: "INTEGER", nullable: false),
                LoggedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_SleepLogs", log => log.Id));

        migrationBuilder.CreateTable(
            name: "ActivityLogs",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                Date = table.Column<DateOnly>(type: "TEXT", nullable: false),
                Type = table.Column<int>(type: "INTEGER", nullable: false),
                DurationMinutes = table.Column<int>(type: "INTEGER", nullable: false),
                Steps = table.Column<int>(type: "INTEGER", nullable: false),
                LoggedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_ActivityLogs", log => log.Id));

        migrationBuilder.CreateIndex(
            name: "IX_SleepLogs_Date",
            table: "SleepLogs",
            column: "Date");
        migrationBuilder.CreateIndex(
            name: "IX_ActivityLogs_Date",
            table: "ActivityLogs",
            column: "Date");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "ActivityLogs");
        migrationBuilder.DropTable(name: "SleepLogs");
    }
}
