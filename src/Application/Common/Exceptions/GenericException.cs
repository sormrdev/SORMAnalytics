namespace SORMAnalytics.Application.Common.Exceptions;

public class GenericException : Exception
{
    public int StatusCode { get; }

    public GenericException(int code, string description) : base(description)
    {
        StatusCode = code;
    }
}