using Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

using SORMAnalytics.Domain.Entities;

namespace SORMAnalytics.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<PriceCandle> PriceCandles {get;}
    DbSet<AssetToFetch> AssetsToFetch {get;}
    DbSet<User> Users {get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    DatabaseFacade Database { get; }

}
