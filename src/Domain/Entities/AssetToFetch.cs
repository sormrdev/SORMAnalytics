namespace SORMAnalytics.Domain.Entities;
public record AssetToFetch
{
    public string Symbol { get; set; } = string.Empty;
}