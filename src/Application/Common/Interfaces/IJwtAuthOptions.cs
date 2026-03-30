
namespace Application.Common.Interfaces;
public interface IJwtAuthOptions
{
    string Issuer { get; init; } 
    string Audience { get; init; } 
    string Key { get; init; }
    int ExpirationInMinutes { get; init; }
    public int RefreshTokenExpirationInDays { get; init; }
}
