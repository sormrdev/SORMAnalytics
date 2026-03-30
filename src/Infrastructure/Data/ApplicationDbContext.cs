using System.Reflection;

using Domain.Entities;

using Microsoft.EntityFrameworkCore;
using SORMAnalytics.Application.Common.Interfaces;
using SORMAnalytics.Domain.Entities;

namespace SORMAnalytics.Infrastructure.Data;

public partial class ApplicationDbContext: DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options) { }
    
    public DbSet<PriceCandle> PriceCandles => Set<PriceCandle>();
    public DbSet<AssetToFetch> AssetsToFetch => Set<AssetToFetch>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema(Schemas.Candles);
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
