namespace SORMAnalytics.Application.Common.Exceptions;

public class GoldenCrossNotFoundException : Exception
{
    public GoldenCrossNotFoundException(string symbol) : base($"Couldn't find any golden crosses for asset with symbol {symbol}."){}
}
