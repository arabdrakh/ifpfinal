using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using MoodLog.Infrastructure.Data;

namespace MoodLog.Infrastructure.Data.Migrations;

[DbContext(typeof(MoodLogDbContext))]
[Migration("20261113000000_InitialCreate")]
public sealed class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "MoodEntries",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                Date = table.Column<DateOnly>(type: "TEXT", nullable: false),
                Level = table.Column<int>(type: "INTEGER", nullable: false),
                Tags = table.Column<string>(type: "TEXT", nullable: false),
                Note = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_MoodEntries", entry => entry.Id));

        migrationBuilder.CreateTable(
            name: "WaterLogs",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "TEXT", nullable: false),
                Date = table.Column<DateOnly>(type: "TEXT", nullable: false),
                AmountMl = table.Column<int>(type: "INTEGER", nullable: false),
                DailyGoalMl = table.Column<int>(type: "INTEGER", nullable: false),
                LoggedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_WaterLogs", log => log.Id));

        migrationBuilder.CreateIndex(
            name: "IX_MoodEntries_Date",
            table: "MoodEntries",
            column: "Date");
        migrationBuilder.CreateIndex(
            name: "IX_WaterLogs_Date",
            table: "WaterLogs",
            column: "Date");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "MoodEntries");
        migrationBuilder.DropTable(name: "WaterLogs");
    }
}
