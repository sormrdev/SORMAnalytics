namespace Application.DTOs.Auth;

public record TokenRequest(string UserId, string Email, IList<string> Roles);
