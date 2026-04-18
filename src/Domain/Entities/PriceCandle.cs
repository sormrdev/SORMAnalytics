namespace SORMAnalytics.Domain.Entities;

public class PriceCandle
{
    public int PriceCandleId {get; init;}
    public string Symbol {get; init;} = string.Empty;
    public decimal Open { get; init; }
    public decimal High { get; init; }
    public decimal Low { get; init; }
    public decimal Close { get; init; }
    public long Volume { get; init; }
    public DateOnly Timestamp { get; init; }
    public decimal? MA50 { get; set; }
    public decimal? MA200 { get; set; }

    private PriceCandle(
      string symbol,
      decimal open,
      decimal high,
      decimal low,
      decimal close,
      long volume,
      DateOnly timestamp)
    {
        if (high < low)
            throw new ArgumentException("High must be >= Low");

        if (open < low || open > high)
            throw new ArgumentException("Open must be within High/Low range");

        if (close < low || close > high)
            throw new ArgumentException("Close must be within High/Low range");

        if (volume < 0)
            throw new ArgumentException("Volume cannot be negative");

        Symbol = symbol;
        Open = open;
        High = high;
        Low = low;
        Close = close;
        Volume = volume;
        Timestamp = timestamp;
    }

    public static PriceCandle Create(
        string symbol,
        decimal open,
        decimal high,
        decimal low,
        decimal close,
        long volume,
        DateOnly timestamp)
    {
        return new PriceCandle(symbol, open, high, low, close, volume, timestamp);
    }

    // Behavior example
    public void UpdateMovingAverages(decimal? ma50, decimal? ma200)
    {
        if (ma50 < 0 || ma200 < 0)
            throw new ArgumentException("MA cannot be negative");

        MA50 = ma50;
        MA200 = ma200;
    }
}
