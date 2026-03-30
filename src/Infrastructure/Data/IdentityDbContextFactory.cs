using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Data;
public class IdentityDbContextFactory : IDesignTimeDbContextFactory<ApplicationIdentityDbContext>
{
    public ApplicationIdentityDbContext CreateDbContext(string[] args)
    {
        var webProjectPath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "..",
            "Web"
        );

        var configuration = new ConfigurationBuilder()
            .SetBasePath(webProjectPath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{GetEnvironment()}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        var connString = configuration.GetConnectionString("SORMADb");

        if (string.IsNullOrEmpty(connString))
        {
            throw new InvalidOperationException("Connection string 'SORMADb' not found in appsettings.");
        }

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationIdentityDbContext>();
        optionsBuilder.UseNpgsql(connString, npsqlOptions =>

        npsqlOptions
            .MigrationsHistoryTable(HistoryRepository.DefaultTableName, "identity"))
            .UseSnakeCaseNamingConvention();

        return new ApplicationIdentityDbContext(optionsBuilder.Options);
    }

    private static string GetEnvironment()
    {
        return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
               ?? "Development";
    }
}
