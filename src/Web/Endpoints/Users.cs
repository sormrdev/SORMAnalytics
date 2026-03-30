using System.Security.Claims;

using Application.DTOs.Users;
using Application.PriceCandles.Queries.Users;

using Microsoft.AspNetCore.Http.HttpResults;

using SORMAnalytics.Application.Common.Exceptions;

using Web.Services;

namespace Web.Endpoints;

public class Users : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet(GetUser);
        groupBuilder.MapGet(GetCurrentUser, "me");
    }
    public async Task<Ok<UserDto>> GetUser(ISender sender, [AsParameters] GetUserQuery query)
    {
        var result = await sender.Send(query);
        return TypedResults.Ok(result);
    }
    public async Task<Ok<UserDto>> GetCurrentUser(ISender sender, IHttpContextAccessor httpContextAccessor, UserContext userContext)
    {
        string? userId = await userContext.GetUserIdAsync();

        if (string.IsNullOrEmpty(userId))
            throw new GenericException(401, "Unauthorized");

        var query = new GetCurrentUserQuery(userId);

        var result = await sender.Send(query);

        return TypedResults.Ok(result);
    }
}
