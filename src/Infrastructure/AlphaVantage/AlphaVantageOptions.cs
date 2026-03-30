using System;

namespace SORMAnalytics.Infrastructure.AlphaVantage;

public class AlphaVantageOptions
{
    public const string SectionName = "AlphaVantageOptions";
    public string ApiKey {get; set;} = string.Empty;
    public string BaseUrl {get; set;} = string.Empty;
}
