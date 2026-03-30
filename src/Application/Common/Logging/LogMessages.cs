
using Microsoft.Extensions.Logging;

namespace SORMAnalytics.Application.Common.Logging;
public static partial class LogMessages
{
    [LoggerMessage(
        EventId = 1000,
        Level = LogLevel.Information,
        Message = "Database migrations applied suuccessfully at {Time}")]
    public static partial void MigrateSuccess(
        this ILogger logger,
        DateTime time);

    [LoggerMessage(
        EventId = 1001,
        Level = LogLevel.Error,
        Message = "An error occured while applying database migrations at {Time}")]
    public static partial void MigrateFailed(
        this ILogger logger,
        DateTime time);

    [LoggerMessage(
       EventId = 1002,
       Level = LogLevel.Information,
       Message = "Found candles, count: {candlesCount}, minDate: {minDate}, maxDate: {maxDate}")]
    public static partial void FoundCandles(
       this ILogger logger,
       int candlesCount,
       DateOnly minDate,
       DateOnly maxDate);
}
