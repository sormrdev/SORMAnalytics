using System.Reflection.Metadata;

using SORMAnalytics.Domain.Entities;
using SORMAnalytics.Domain.Interfaces;

namespace SORMAnalytics.Domain.Services;
public class BatchAnalysisService : IBatchAnalyser
{
    public (IEnumerable<PriceCandle>?, IEnumerable<PriceCandle>?) Analyse(IEnumerable<PriceCandle> filteredPriceCandles, BatchAnalysisCondition condition)
    {
        List<PriceCandle> result = new List<PriceCandle>();
        List<PriceCandle> candlesInvolved = new List<PriceCandle>();

        var candleDict = filteredPriceCandles.ToDictionary(c => c.Timestamp);

        foreach (var candle in filteredPriceCandles)
        {
            bool allChecksPassed = true;

            var involvedThisResult = new HashSet<PriceCandle>();

            foreach (var body in condition.rules)
            {
                (var firstValue, var firstCandleInvolved) = GetValue(body.firstValue, candleDict, candle.Timestamp, body.firstValueDayOffset);
                (var secondValue, var secondCandleInvolved) = GetValue(body.secondValue, candleDict, candle.Timestamp, body.secondValueDayOffset);

                bool compareResult = body.comparisonOperator switch
                {
                    ComparisonOperator.LessThan => firstValue < secondValue,
                    ComparisonOperator.GreaterThan => firstValue > secondValue,
                    ComparisonOperator.LessThanOrEqual => firstValue <= secondValue,
                    ComparisonOperator.GreaterThanOrEqual => firstValue >= secondValue,
                    ComparisonOperator.Equal => firstValue == secondValue,
                    _ => false
                };

                if (!compareResult)
                {
                    allChecksPassed = false;
                    break;
                }

                if (firstCandleInvolved != null && firstCandleInvolved != candle)
                    involvedThisResult.Add(firstCandleInvolved);
                if (secondCandleInvolved != null && secondCandleInvolved != candle)
                    involvedThisResult.Add(secondCandleInvolved);
            }
            if (allChecksPassed)
            {
                result.Add(candle);
                candlesInvolved.AddRange(involvedThisResult);
            }
        }
        return (result, candlesInvolved);
    }
    private (decimal?, PriceCandle?) GetValue(AnalysableValues valueType,
                         Dictionary<DateOnly, PriceCandle> candles,
                         DateOnly referenceDate,
                         int offsetDays)
    {
        var targetDate = referenceDate.AddDays(offsetDays);
        if (!candles.TryGetValue(targetDate, out var targetCandle))
            return (null, null);

        return (valueType switch
        {
            AnalysableValues.MA50 => targetCandle.MA50,
            AnalysableValues.MA200 => targetCandle.MA200,
            AnalysableValues.Open => targetCandle.Open,
            AnalysableValues.High => targetCandle.High,
            AnalysableValues.Low => targetCandle.Low,
            AnalysableValues.Close => targetCandle.Close,
            AnalysableValues.Volume => targetCandle.Volume,
            _ => null
        }, targetCandle);
    }
}
