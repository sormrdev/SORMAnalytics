using Application.DTOs.Auth;

using Domain.Entities;

namespace Application.DTOs.Users;
public static class UserMappings
{
    public static User ToEntity(this RegisterUserDto dto, string IdentityId)
    {
        return User.Create
        (
            $"u_{Guid.CreateVersion7()}",
            dto.Name,
            dto.Email,
            IdentityId
        );
    }
}
