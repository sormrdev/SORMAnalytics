using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

using Microsoft.Extensions.Options;

using SORMAnalytics.Application.Common.Interfaces;
using SORMAnalytics.Domain.Entities;
using SORMAnalytics.Infrastructure.Exceptions;

namespace SORMAnalytics.Infrastructure.FMP;
public class FmpDailyService : IPriceCandleProvider
{
    private readonly HttpClient _httpClient;
    private readonly FmpOptions _options;
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public FmpDailyService(HttpClient httpClient, IOptions<FmpOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<IEnumerable<PriceCandle>> GetPriceCandleAsync(string symbol, CancellationToken cancellationToken = default)
    {
        var url = $"{_options.BaseUrl}{_options.DailyQuery}?symbol={symbol}&apikey={_options.ApiKey}";

        var response = await _httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var avResponse = await JsonSerializer.DeserializeAsync<List<PriceCandleData>>(stream, _jsonOptions, cancellationToken);

        if (avResponse?.Count == 0)
            throw new AlphaVantageResponseException(symbol);

        var result = new List<PriceCandle>();

        foreach (var candle in avResponse!)
        {
            result.Add(PriceCandle.Create
            (
                symbol,
                candle.Open01,
                candle.High02,
                candle.Low03,
                candle.Close04,
                candle.Volume05,
                DateOnly.Parse(candle.Date06, CultureInfo.InvariantCulture)
            ));
        }
        return result;
    }
    private record PriceCandleData
    {
        [JsonPropertyName("adjOpen")] public decimal Open01 { get; init; } 
        [JsonPropertyName("adjHigh")] public decimal High02 { get; init; } 
        [JsonPropertyName("adjLow")] public decimal Low03 { get; init; }
        [JsonPropertyName("adjClose")] public decimal Close04 { get; init; } 
        [JsonPropertyName("volume")] public long Volume05 { get; init; }
        [JsonPropertyName("date")] public string Date06 { get; init; } = string.Empty;
    }
}