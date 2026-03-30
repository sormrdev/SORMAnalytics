namespace Application.DTOs.PriceCandles;
public record PriceCandleParameters(string Symbol, string? Fields, int Page = 1, int PageSize = 10);