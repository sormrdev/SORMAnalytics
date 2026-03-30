using Application.PriceCandles.Commands.Auth;

namespace Web.Endpoints;

public class Authentication : EndpointGroupBase
{
    public override string? GroupName => "auth";
    public override void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPost(Register, "register");
        groupBuilder.MapPost(Login, "login");
        groupBuilder.MapPost(RefreshTokens, "refresh");
    }
    public async Task<IResult> Register(ISender sender, [AsParameters] RegisterUser command)
    {
        var result = await sender.Send(command);
        return result;
    }
    public async Task<IResult> Login(ISender sender, [AsParameters] LoginUser command)
    {
        var result = await sender.Send(command);
        return result;
    }
    public async Task<IResult> RefreshTokens(ISender sender, [AsParameters] RefreshTokens command)
    {
        var result = await sender.Send(command);
        return result;
    }
}
