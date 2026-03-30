using Application.DTOs.Auth;

namespace Application.Common.Interfaces;
public interface ITokenProvider
{
    AccessTokensDto Create(TokenRequest tokenRequest);
}
