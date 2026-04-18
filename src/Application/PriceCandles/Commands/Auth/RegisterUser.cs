using Application.DTOs.Auth;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using SORMAnalytics.Application.Common.Interfaces;
using Application.Common.Interfaces;
using Microsoft.AspNetCore.Identity;
using Application.DTOs.Users;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Application.PriceCandles.Commands.Auth;
public record RegisterUser(RegisterUserDto registerUserDto) : IRequest<IResult> { }

public class RegisterUserHandler : IRequestHandler<RegisterUser, IResult>
{
    private readonly IApplicationDbContext _applicationContext;
    private readonly IIdentityDbContext _identityContext;
    private readonly ITokenProvider _tokenProvider;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IJwtAuthOptions _jwtAuthOptions;
    public RegisterUserHandler
        (UserManager<IdentityUser> userManager, 
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
    public async Task<IResult> Handle(RegisterUser request, CancellationToken cancellationToken)
    {
        using IDbContextTransaction transaction = await _identityContext.Database.BeginTransactionAsync(cancellationToken);
        _applicationContext.Database.SetDbConnection(_identityContext.Database.GetDbConnection());
        await _applicationContext.Database.UseTransactionAsync(transaction.GetDbTransaction(), cancellationToken);

        var identityUser = new IdentityUser
        {
            UserName = request.registerUserDto.Name,
            Email = request.registerUserDto.Email,
        };

        IdentityResult identityResult = await _userManager.CreateAsync(identityUser, request.registerUserDto.Password);

        if (!identityResult.Succeeded)
        {
            var extensions = new Dictionary<string, object?>
            {
                { "Errors", identityResult.Errors.ToDictionary(e => e.Code, e => e.Description) }
            };
            return Results.Problem(statusCode: StatusCodes.Status400BadRequest, detail: "User registration failed", extensions: extensions);
        }

        await _userManager.AddToRoleAsync(identityUser, "User");

        User user = request.registerUserDto.ToEntity(identityUser.Id);

        _applicationContext.Users.Add(user);

        await _applicationContext.SaveChangesAsync(cancellationToken);

        var roles = await _userManager.GetRolesAsync(identityUser);
        TokenRequest tokenRequest = new(identityUser.Id, identityUser.Email, roles);
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

        await transaction.CommitAsync(cancellationToken);

        return Results.Ok(accessTokens);
    }
}