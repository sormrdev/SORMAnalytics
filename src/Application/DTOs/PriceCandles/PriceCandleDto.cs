using System.Text.Json.Serialization;

using AutoMapper;

using SORMAnalytics.Domain.Entities;

namespace Application.DTOs.PriceCandles;

public record PriceCandleDto
{
    public decimal Open { get; init; }
    public decimal High { get; init; }
    public decimal Low { get; init; }
    public decimal Close { get; init; }
    public long Volume { get; init; }
    public DateOnly Timestamp { get; init; }

    [JsonPropertyName("ma50")]
    public decimal? MA50 { get; init; }

    [JsonPropertyName("ma200")]
    public decimal? MA200 { get; init; }
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<PriceCandle, PriceCandleDto>();

        }
    }
}