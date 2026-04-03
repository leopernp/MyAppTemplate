using DamayanFS.Contract.DTO;
using DamayanFS.Contract.Enums;
using DamayanFS.Contract.Interfaces;
using DamayanFS.Contract.Models.Auth;

namespace DamayanFS.App.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly ILogger<AuthenticationService> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IUserActivityLogService _userActivityLogService;

    private const int MAX_FAILED_ATTEMPTS = 3;
    private const int LOCKOUT_DURATION_MINUTES = 5;

    public AuthenticationService(
        ILogger<AuthenticationService> logger,
        IUserRepository userRepository,
        IUserActivityLogService userActivityLogService)
    {
        _logger = logger;
        _userRepository = userRepository;
        _userActivityLogService = userActivityLogService;
    }

    public async Task<LoginResult> AuthenticateAsync(
        string username,
        string password,
        string? ipAddress = null,
        string? browser = null)
    {
        try
        {
            var user = await _userRepository.GetByUsernameForAuthAsync(username);

            // Unknown username — generic invalid credentials, no lockout tracking
            if (user == null)
            {
                await _userActivityLogService.LogAsync(
                    UserActivityAction.Login,
                    $"Failed login attempt for unknown username \"{username}\"",
                    null,
                    ipAddress,
                    browser);

                return LoginResult.InvalidCredentials();
            }

            // Inactive account
            if (!user.IsActive)
            {
                await _userActivityLogService.LogAsync(
                    UserActivityAction.Login,
                    $"Failed login attempt for inactive user {user.Username}",
                    null,
                    ipAddress,
                    browser);

                return LoginResult.Inactive();
            }

            // Check lockout — if still within lockout window
            if (user.IsLockedOut)
            {
                await _userActivityLogService.LogAsync(
                    UserActivityAction.Login,
                    $"Login attempt blocked — user {user.Username} is locked out until {user.LockoutEnd:u}",
                    null,
                    ipAddress,
                    browser);

                return LoginResult.LockedOut(user.LockoutEnd!.Value);
            }

            // Verify password
            if (!VerifyPassword(password, user.Password))
            {
                var newFailedAttempts = user.FailedLoginAttempts + 1;
                DateTime? lockoutEnd = null;

                if (newFailedAttempts >= MAX_FAILED_ATTEMPTS)
                {
                    lockoutEnd = DateTime.UtcNow.AddMinutes(LOCKOUT_DURATION_MINUTES);

                    _logger.LogWarning(
                        "User {Username} has been locked out after {Attempts} failed attempts",
                        user.Username, newFailedAttempts);

                    await _userActivityLogService.LogAsync(
                        UserActivityAction.Login,
                        $"User {user.Username} locked out after {newFailedAttempts} failed login attempts",
                        null,
                        ipAddress,
                        browser);

                    await _userRepository.UpdateLockoutAsync(user.Id, 0, lockoutEnd);
                    return LoginResult.LockedOut(lockoutEnd.Value);
                }

                await _userActivityLogService.LogAsync(
                    UserActivityAction.Login,
                    $"Failed login attempt {newFailedAttempts} of {MAX_FAILED_ATTEMPTS} for user {user.Username}",
                    null,
                    ipAddress,
                    browser);

                await _userRepository.UpdateLockoutAsync(user.Id, newFailedAttempts, null);
                return LoginResult.InvalidCredentials();
            }

            // Successful login — reset lockout state
            await _userRepository.UnlockAsync(user.Id);

            // Reload user after unlock to get clean state
            var authenticatedUser = await _userRepository.GetByIdAsync(user.Id);

            await _userActivityLogService.LogAsync(
                UserActivityAction.Login,
                $"User {user.Username} logged in successfully",
                user.Id,
                ipAddress,
                browser);

            return LoginResult.Success(authenticatedUser!);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during authentication for username {Username}", username);
            return LoginResult.InvalidCredentials();
        }
    }

    public async Task LogoutAsync(
        int userId,
        string username,
        string? ipAddress = null,
        string? browser = null)
    {
        await _userActivityLogService.LogAsync(
            UserActivityAction.Logout,
            $"User {username} logged out",
            userId,
            ipAddress,
            browser);
    }

    public async Task<UserDto?> GetAuthenticatedUserAsync(int userId) =>
        await _userRepository.GetByIdAsync(userId);

    public string HashPassword(string password) =>
        BCrypt.Net.BCrypt.HashPassword(password);

    public bool VerifyPassword(string password, string passwordHash)
    {
        try
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while verifying password");
            return false;
        }
    }
}