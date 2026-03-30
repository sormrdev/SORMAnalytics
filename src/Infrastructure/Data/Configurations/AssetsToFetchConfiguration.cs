using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SORMAnalytics.Domain.Entities;

namespace Infrastructure.Data.Configurations;

public class AssetsToFetchConfiguration : IEntityTypeConfiguration<AssetToFetch>
{
    public void Configure(EntityTypeBuilder<AssetToFetch> builder)
    {
        builder.HasKey(x => x.Symbol);

        builder.HasData(
                new AssetToFetch {Symbol = "TSLA" },
                new AssetToFetch {Symbol = "AAPL"}
        ); 
    }
}
