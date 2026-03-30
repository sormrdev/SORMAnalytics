using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SORMAnalytics.Application.Common.Exceptions;
using SORMAnalytics.Infrastructure.Exceptions;

namespace SORMAnalytics.Web.Infrastructure;

public class CustomExceptionHandler : IExceptionHandler
{
    private readonly Dictionary<Type, Func<HttpContext, Exception, Task>> _exceptionHandlers;
    public CustomExceptionHandler()
    {
        _exceptionHandlers = new()
            {
                { typeof(AlphaVantageResponseException), HandleAlphaVantageResponseException },
                { typeof(GoldenCrossNotFoundException), HandleGoldenCrossNotFoundException },
                { typeof(FluentValidation.ValidationException), HandleValidationException },
                { typeof(SortingException), HandleSortingException },
                { typeof(GenericException), HandleGenericException }
            };
    }
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var exceptionType = exception.GetType();

        if (_exceptionHandlers.TryGetValue(exceptionType, out Func<HttpContext, Exception, Task>? value))
        {
            await value.Invoke(httpContext, exception);
            return true;
        }

        await HandleUnknownException(httpContext);

        return false;
    }
    private async Task HandleAlphaVantageResponseException(HttpContext httpContext, Exception ex)
    {
        var exception = (AlphaVantageResponseException)ex;

        httpContext.Response.StatusCode = StatusCodes.Status404NotFound;

        await httpContext.Response.WriteAsJsonAsync(new ValidationProblemDetails()
        {
            Status = StatusCodes.Status404NotFound,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            Title = "The specified resource was not found.",
            Detail = exception.Message
        });
    }
     private async Task HandleGoldenCrossNotFoundException(HttpContext httpContext, Exception ex)
     {
        var exception = (GoldenCrossNotFoundException)ex;

        httpContext.Response.StatusCode = StatusCodes.Status404NotFound;

        await httpContext.Response.WriteAsJsonAsync(new ValidationProblemDetails()
        {
            Status = StatusCodes.Status404NotFound,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
            Title = "The specified resource was not found.",
            Detail = exception.Message
        });
     }
    private async Task HandleValidationException(HttpContext httpContext, Exception ex)
    {
        var exception = (FluentValidation.ValidationException)ex;
        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        await httpContext.Response.WriteAsJsonAsync(new ValidationProblemDetails()
        {
            Status = StatusCodes.Status400BadRequest,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "One or more validation errors occurred.",
            Detail = "See the errors property for details.",
            Errors = exception.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())
        });
    }
    private async Task HandleUnknownException(HttpContext httpContext)
    {
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
            Title = "An unexpected error occurred.",
            Detail = "The server encountered an unexpected condition."
        });
    }
    private async Task HandleSortingException(HttpContext httpContext, Exception ex)
    {
        var exception = (SortingException)ex;

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

        await httpContext.Response.WriteAsJsonAsync(new ValidationProblemDetails()
        {
            Status = StatusCodes.Status400BadRequest,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "Parameter error.",
            Detail = exception.Message
        });
    }
    private async Task HandleGenericException(HttpContext httpContext, Exception ex)
    {
        var exception = (GenericException)ex;

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails()
        {
            Status = exception.StatusCode,
            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
            Title = "Bad Request.",
            Detail = exception.Message
        });
    }
}
