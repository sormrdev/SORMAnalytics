using Hangfire;
using Hangfire.PostgreSql;

using SORMAnalytics.Infrastructure.Scheduling;

namespace Web.Services;

public static class HangfireService
{
    public static void AddHangfire(this WebApplicationBuilder builder)
    {
        builder.Services.AddHangfire(config =>
            config.UsePostgreSqlStorage(options =>
                options.UseNpgsqlConnection(
                    builder.Configuration.GetConnectionString("SORMADb"))));

        builder.Services.AddHangfireServer();
    }

    public static void ScheduleJobs(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var recurringJobManager = scope.ServiceProvider
            .GetRequiredService<IRecurringJobManager>();

        recurringJobManager.AddOrUpdate<DailyUpdateJob>(
            "daily-price-update",
            job => job.Execute(),
            "0 12 * * *"
        );
    }
}
