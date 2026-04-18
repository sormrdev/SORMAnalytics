using Application.Common.Interfaces;
using Application.DTOs.Auth;

using AutoMapper;

using Domain.Entities;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using SORMAnalytics.Application.Common.Interfaces;

namespace Application.PriceCandles.Commands.Auth;
public record RefreshTokens(RefreshTokenDto refreshTokenDto) : IRequest<IResult> { }

public class RefreshTokensHandler : IRequestHandler<RefreshTokens, IResult>
{
    private readonly IApplicationDbContext _applicationContext;
    private readonly IIdentityDbContext _identityContext;
    private readonly ITokenProvider _tokenProvider;
    private readonly IJwtAuthOptions _jwtAuthOptions;
    private readonly UserManager<IdentityUser> _userManager;
    public RefreshTokensHandler(
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
    public async Task<IResult> Handle(RefreshTokens request, CancellationToken cancellationToken)
    {
        RefreshToken? refreshToken = await _identityContext.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == request.refreshTokenDto.RefreshToken, cancellationToken);

        if (refreshToken is null)
        {
            return TypedResults.Unauthorized();
        }

        if (refreshToken.Expires < DateTime.UtcNow)
        {
            return TypedResults.Unauthorized();
        }

        var roles = await _userManager.GetRolesAsync(refreshToken.User);

        var tokenRequest = new TokenRequest(refreshToken.User.Id, refreshToken.User.Email!, roles);
        var accessTokens = _tokenProvider.Create(tokenRequest);

        refreshToken.Token = accessTokens.RefreshToken;
        refreshToken.Expires = DateTime.UtcNow.AddDays(_jwtAuthOptions.RefreshTokenExpirationInDays);

        await _identityContext.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok(accessTokens);
    }
}