using SORMAnalytics.Domain.Entities;
using SORMAnalytics.Domain.Interfaces;

namespace SORMAnalytics.Domain.Services;

public class CalculatePriceCandles : ICalculatePriceCandle
{
   public void Calculate(IEnumerable<PriceCandle> priceCandles)
    {
        var priceList = priceCandles.OrderBy(p => p.Timestamp).ToList();
        int count = priceList.Count;

        decimal sum50; 
        decimal sum200; 

        for (int i = 0; i < count; i++)
        {
            var candle = priceList[i];

            decimal? ma50 = null;
            if (i >= 50)
            {
                sum50 = 0;
                for (int j = i - 50; j < i; j++) sum50 += priceList[j].Close;
                ma50 = sum50 / 50;
            }

            decimal? ma200 = null;
            if (i >= 200)
            {
                sum200 = 0;
                for (int j = i - 200; j < i; j++) sum200 += priceList[j].Close;
                ma200 = sum200 / 200;
            }

            if (candle.MA50 == null || candle.MA200 == null)
            {
               candle.MA50 = ma50;
               candle.MA200 = ma200;
            }
        }
    }
}
