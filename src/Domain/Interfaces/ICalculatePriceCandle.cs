using SORMAnalytics.Domain.Entities;

namespace SORMAnalytics.Domain.Interfaces;

public interface ICalculatePriceCandle
{
    public void Calculate(IEnumerable<PriceCandle> priceCandles);
}
