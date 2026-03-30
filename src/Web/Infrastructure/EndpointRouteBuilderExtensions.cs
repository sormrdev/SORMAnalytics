using System.Diagnostics.CodeAnalysis;

namespace SORMAnalytics.Web.Infrastructure;

public static class EndpointRouteBuilderExtensions
{
    public static RouteHandlerBuilder MapGet(this IEndpointRouteBuilder builder, Delegate handler, [StringSyntax("Route")] string pattern = "")
    {
        return builder.MapGet(pattern, handler)
              .WithName(handler.Method.Name);
    }
    public static RouteHandlerBuilder MapPost(this IEndpointRouteBuilder builder, Delegate handler, [StringSyntax("Route")] string pattern = "")
    {
        return builder.MapPost(pattern, handler)
              .WithName(handler.Method.Name);
    }
}
