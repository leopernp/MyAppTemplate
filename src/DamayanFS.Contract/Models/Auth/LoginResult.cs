using MyAppTemplate.Contract.DTO;

namespace MyAppTemplate.Contract.Models.Auth;

public class LoginResult
{
    public bool IsSuccess { get; private set; }
    public bool IsLockedOut { get; private set; }
    public bool IsInactive { get; private set; }
    public bool IsInvalidCredentials { get; private set; }
    public DateTime? LockoutEnd { get; private set; }
    public UserDto? User { get; private set; }

    public int RemainingLockoutSeconds => IsLockedOut && LockoutEnd.HasValue
        ? (int)Math.Ceiling((LockoutEnd.Value - DateTime.UtcNow).TotalSeconds)
        : 0;

    public string ErrorMessage => IsLockedOut
        ? $"Your account is locked. Please try again in {RemainingLockoutSeconds} seconds."
        : IsInactive
            ? "Your account is inactive. Please contact your administrator."
            : IsInvalidCredentials
                ? "Invalid username or password."
                : string.Empty;

    // Static factory methods — clean construction, no public setters
    public static LoginResult Success(UserDto user) => new()
    {
        IsSuccess = true,
        User = user
    };

    public static LoginResult LockedOut(DateTime lockoutEnd) => new()
    {
        IsLockedOut = true,
        LockoutEnd = lockoutEnd
    };

    public static LoginResult Inactive() => new()
    {
        IsInactive = true
    };

    public static LoginResult InvalidCredentials() => new()
    {
        IsInvalidCredentials = true
    };
}