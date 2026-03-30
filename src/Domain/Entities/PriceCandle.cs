namespace SORMAnalytics.Domain.Entities;

public record PriceCandle
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
}