using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SORMAnalytics.Domain.Entities;

namespace Infrastructure.Data.Configurations;

public class PriceCandleConfiguration : IEntityTypeConfiguration<PriceCandle>
{
    public void Configure(EntityTypeBuilder<PriceCandle> builder)
    {
        builder.HasKey(x => x.PriceCandleId);
        builder
            .HasIndex(x => new {x.Symbol, x.Timestamp})
            .IsUnique();
    }
}
