using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using MoodLog.Infrastructure.Data;

namespace MoodLog.Infrastructure.Data.Migrations;

[DbContext(typeof(MoodLogDbContext))]
[Migration("20261113000200_AddProfileSettings")]
public sealed class AddProfileSettings : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "ProfileSettings",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                DisplayName = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                DailyWaterGoalMl = table.Column<int>(type: "INTEGER", nullable: false),
                DailySleepGoalMinutes = table.Column<int>(type: "INTEGER", nullable: false),
                DailyStepGoal = table.Column<int>(type: "INTEGER", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_ProfileSettings", settings => settings.Id));
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "ProfileSettings");
    }
}
