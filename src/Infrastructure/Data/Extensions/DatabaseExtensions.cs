using Infrastructure.Data;

using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SORMAnalytics.Application.Common.Logging;

namespace SORMAnalytics.Infrastructure.Data.Extensions;
public static class DatabaseExtensions
{
    public static async Task ApplyMigrationsAsync(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        await using ApplicationDbContext applicationDbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await using ApplicationIdentityDbContext identityDbContext = scope.ServiceProvider.GetRequiredService<ApplicationIdentityDbContext>();

        try
        {
            await applicationDbContext.Database.MigrateAsync();

            await identityDbContext.Database.MigrateAsync();

            app.Logger.MigrateSuccess(DateTime.UtcNow);
        }
        catch (Exception)
        {
            app.Logger.MigrateFailed(DateTime.UtcNow);
            throw;
        }
    }
}
