using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MoodLog.Infrastructure.Data;
using MoodLog.Core.Interfaces;
using MoodLog.Core.Services;

namespace MoodLog.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddMoodLogInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("MoodLog")
            ?? "Data Source=moodlog.db";

        services.AddDbContext<MoodLogDbContext>(options =>
            options.UseSqlite(connectionString));
        services.AddScoped<IMoodEntryRepository, EfMoodEntryRepository>();
        services.AddScoped<MoodJournalService>();

        return services;
    }
}
