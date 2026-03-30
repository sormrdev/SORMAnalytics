using Application.Common.Interfaces;
using Application.DTOs.Auth;

using AutoMapper;

using Domain.Entities;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

using SORMAnalytics.Application.Common.Interfaces;

namespace Application.PriceCandles.Commands.Auth;
public record LoginUser(LoginUserDto loginUserDto) : IRequest<IResult> { }

public class LoginUserHandler : IRequestHandler<LoginUser, IResult>
{
    private readonly IApplicationDbContext _applicationContext;
    private readonly IIdentityDbContext _identityContext;
    private readonly ITokenProvider _tokenProvider;
    private readonly IJwtAuthOptions _jwtAuthOptions;
    private readonly UserManager<IdentityUser> _userManager;
    public LoginUserHandler(
        UserManager<IdentityUser> userManager, 
        IApplicationDbContext applicationContext, 
        IIdentityDbContext identityContext, 
        ITokenProvider tokenProvider,
        IJwtAuthOptions options)
    {
        _jwtAuthOptions = options;
        _applicationContext = applicationContext;
        _identityContext = identityContext;
        _tokenProvider = tokenProvider;
        _userManager = userManager;
    }
    public async Task<IResult> Handle(LoginUser request, CancellationToken cancellationToken)
    {
        IdentityUser? identityUser = await _userManager.FindByEmailAsync(request.loginUserDto.Email);
        if (identityUser == null || !await _userManager.CheckPasswordAsync(identityUser, request.loginUserDto.Password))
        {
            return Results.Unauthorized();
        }
        
        var tokenRequest = new TokenRequest(identityUser.Id, identityUser.Email!);
        var accessTokens = _tokenProvider.Create(tokenRequest);

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = identityUser.Id,
            Token = accessTokens.RefreshToken,
            Expires = DateTime.UtcNow.AddDays(_jwtAuthOptions.RefreshTokenExpirationInDays)
        };

        _identityContext.RefreshTokens.Add(refreshToken);

        await _identityContext.SaveChangesAsync(cancellationToken);

        return Results.Ok(accessTokens);
    }
}