using System;

namespace SORMAnalytics.Infrastructure.Exceptions;

public class AlphaVantageResponseException : Exception
{
    public AlphaVantageResponseException(string symbol) : base($"Couldn't fetch data for asset with symbol {symbol}."){}
}
