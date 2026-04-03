using MyAppTemplate.Contract.DTO;
using MyAppTemplate.Contract.Models.Auth;

namespace MyAppTemplate.Contract.Interfaces;

public interface IAuthenticationService
{
    Task<LoginResult> AuthenticateAsync(string username, string password, string? ipAddress = null, string? browser = null);
    Task LogoutAsync(int userId, string username, string? ipAddress = null, string? browser = null);
    Task<UserDto?> GetAuthenticatedUserAsync(int userId);
    string HashPassword(string password);
    bool VerifyPassword(string password, string passwordHash);
}