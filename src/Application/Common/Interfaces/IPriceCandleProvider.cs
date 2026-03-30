using System;
using SORMAnalytics.Domain.Entities;

namespace SORMAnalytics.Application.Common.Interfaces;

public interface IPriceCandleProvider
{
    Task<IEnumerable<PriceCandle>> GetPriceCandleAsync(string symbol, CancellationToken cancellationToken = default);
}
