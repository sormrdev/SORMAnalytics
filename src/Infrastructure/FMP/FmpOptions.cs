namespace SORMAnalytics.Infrastructure.FMP;
public class FmpOptions
{
    public const string SectionName = "FmpOptions";
    public string ApiKey { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = string.Empty;
    public string DailyQuery { get; set; } = string.Empty;
}
