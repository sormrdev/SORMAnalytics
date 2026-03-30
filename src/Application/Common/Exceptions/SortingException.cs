namespace SORMAnalytics.Application.Common.Exceptions;

public class SortingException : Exception
{
    public SortingException(string sortString) : base($"The provided sort parameter isn't valid: {sortString}."){ }
}