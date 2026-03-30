using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;

namespace SORMAnalytics.Infrastructure.Data;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
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

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseNpgsql(connString, npsqlOptions =>
        // This tells the migration tool: "Look for the history table in 'candles'"
        npsqlOptions
            .MigrationsHistoryTable(HistoryRepository.DefaultTableName, "candles"))
            .UseSnakeCaseNamingConvention();

        return new ApplicationDbContext(optionsBuilder.Options);
    }

    private static string GetEnvironment()
    {
        return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") 
               ?? "Development"; 
    }
}