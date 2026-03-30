using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Globalization;
using SORMAnalytics.Application.Common.Interfaces;
using SORMAnalytics.Infrastructure.AlphaVantage;
using SORMAnalytics.Domain.Entities;
using System.Linq;
using SORMAnalytics.Infrastructure.Exceptions;

namespace SORMAnalytics.Infrastructure.AlphaVantage;

public class AlphaVantageSeriesService : IPriceCandleProvider
{
    private readonly HttpClient _httpClient;
    private readonly AlphaVantageOptions _options;
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public AlphaVantageSeriesService(HttpClient httpClient, IOptions<AlphaVantageOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<IEnumerable<PriceCandle>> GetPriceCandleAsync(string symbol, CancellationToken cancellationToken = default)
    {
        var url = $"{_options.BaseUrl}?function=TIME_SERIES_MONTHLY&symbol={symbol}&apikey={_options.ApiKey}";

        var response = await _httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var avResponse = await JsonSerializer.DeserializeAsync<AlphaVantageGlobalSeriesResponse>(stream, _jsonOptions, cancellationToken);

        if (avResponse?.TimeSeries == null)
            throw new AlphaVantageResponseException(symbol);

        var result = new List<PriceCandle>();
        foreach (var (dateKey, candle) in avResponse.TimeSeries)
        {
            result.Add(new PriceCandle
            {
                Symbol = symbol,
                Open = decimal.Parse(candle.Open01, CultureInfo.InvariantCulture),
                High = decimal.Parse(candle.High02, CultureInfo.InvariantCulture),
                Low = decimal.Parse(candle.Low03, CultureInfo.InvariantCulture),
                Close = decimal.Parse(candle.Close04, CultureInfo.InvariantCulture),
                Volume = long.Parse(candle.Volume05, CultureInfo.InvariantCulture),
                Timestamp = DateOnly.Parse(dateKey, CultureInfo.InvariantCulture)
            });
        }
        return result;
    }

    // Private models only for deserialization (never leave Infrastructure)
    private record AlphaVantageGlobalSeriesResponse
    {
        [JsonPropertyName("Monthly Time Series")]
        public Dictionary<string, PriceCandleData>? TimeSeries { get; init; }
    }

    private record PriceCandleData
    {
        [JsonPropertyName("1. open")] public string Open01 { get; init; } = string.Empty;
        [JsonPropertyName("2. high")] public string High02 { get; init; } = string.Empty;
        [JsonPropertyName("3. low")] public string Low03 { get; init; } = string.Empty;
        [JsonPropertyName("4. close")] public string Close04 { get; init; } = string.Empty;
        [JsonPropertyName("5. volume")] public string Volume05 { get; init; } = string.Empty;
    }
}