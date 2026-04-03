using DamayanFS.Contract.DTO;
using DamayanFS.Contract.Models.ModuleType;
using DamayanFS.Contract.Models.Role;
using DamayanFS.Contract.Models.User;

namespace DamayanFS.Contract.Interfaces;

public interface ISettingsService
{
    #region User Operations
    Task<UserDto?> GetUserByEmailAsync(string email);
    Task<UserDto?> GetUserByIdAsync(int id);
    Task<UserDto?> GetUserByUsernameAsync(string username);
    Task<IEnumerable<UserDto>> InquireUsersAsync(bool? isActive = null, int? roleId = null);
    Task<UserDto> SaveUserAsync(UserUpsertModel model, int? currentUserId, string? ipAddress = null, string? browser = null);
    Task DeleteUserAsync(int id, string? deletedUsername, int? currentUserId, string? ipAddress = null, string? browser = null);
    Task ResetPasswordAsync(int id, int? currentUserId, string? ipAddress = null, string? browser = null);
    Task UnlockUserAsync(int userId, int? currentUserId);
    #endregion

    #region Role Operations
    Task<RoleDto?> GetRoleByIdAsync(int id);
    Task<RoleDto?> GetRoleByNameAsync(string name);
    Task<IEnumerable<RoleDto>> InquireRolesAsync(bool? isActive = null);
    Task<RoleDto> SaveRoleAsync(RoleUpsertModel model, int? currentUserId);
    Task DeleteRoleAsync(int id);
    #endregion

    #region ModuleType Operations
    Task<ModuleTypeDto?> GetModuleTypeByIdAsync(int id);
    Task<IEnumerable<ModuleTypeDto>> InquireModuleTypesAsync(bool? isActive = null);
    Task<ModuleTypeDto> SaveModuleTypeAsync(ModuleTypeUpsertModel model, int? currentUserId);
    Task DeleteModuleTypeAsync(int id);
    #endregion
}