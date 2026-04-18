using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

using Application.Common.Interfaces;
using Application.DTOs.Auth;

using Infrastructure.Settings;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.JWT;

public class TokenProvider(IOptions<JwtAuthOptions> options) : ITokenProvider
{
    private readonly JwtAuthOptions _options = options.Value;
    public AccessTokensDto Create(TokenRequest tokenRequest)
    {
        return new AccessTokensDto(GenerateAccessToken(tokenRequest), GenerateRefreshToken());
    }
    private string GenerateAccessToken(TokenRequest tokenRequest)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        List<Claim> claims =
        [
            new (JwtRegisteredClaimNames.Sub, tokenRequest.UserId),
            new(JwtRegisteredClaimNames.Email, tokenRequest.Email),
        ];

        claims.AddRange(
           tokenRequest.Roles.Select(role =>
               new Claim("role", role)));


        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_options.ExpirationInMinutes),
            SigningCredentials = credentials,
            Issuer = _options.Issuer,
            Audience = _options.Audience
        };

        var handler = new JsonWebTokenHandler();

        string accessToken = handler.CreateToken(tokenDescriptor);

        return accessToken;
    }
    private static string GenerateRefreshToken()
    {
        byte[] randomBytes = RandomNumberGenerator.GetBytes(32);

        return Convert.ToBase64String(randomBytes);
    }
}
