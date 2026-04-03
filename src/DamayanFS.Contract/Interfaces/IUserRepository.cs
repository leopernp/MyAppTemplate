using DamayanFS.Contract.DTO;
using DamayanFS.Contract.Helpers;

namespace DamayanFS.Contract.Interfaces;

public interface IUserRepository
{
    // Read
    Task<UserDto?> GetByEmailAsync(string email);
    Task<UserDto?> GetByIdAsync(int id);
    Task<UserDto?> GetByUsernameAsync(string username);
    Task<UserDto?> GetByUsernameForAuthAsync(string username);
    Task<IEnumerable<UserDto>> InquireAsync(bool? isActive = null, int? roleId = null);

    // Validation
    Task<CustomValidateResult> ValidateAsync(UserDto user);

    // Operations
    Task<UserDto> CreateAsync(UserDto dto, int? currentUserId);
    Task<UserDto> UpdateAsync(UserDto dto, int? currentUserId);
    Task DeleteAsync(int id);

    // Password Operations
    Task UpdatePasswordAsync(int userId, string hashedPassword);

    // Lockout Operations
    Task UpdateLockoutAsync(int userId, int failedAttempts, DateTime? lockoutEnd);
    Task UnlockAsync(int userId);
}