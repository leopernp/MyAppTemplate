using DamayanFS.Contract.DTO;
using DamayanFS.Contract.Models.Module;
using DamayanFS.Contract.Models.ModulePermission;
using DamayanFS.Contract.Models.ModuleType;

namespace DamayanFS.Contract.Interfaces;

public interface IModuleService
{
    #region ModuleType Operations
    Task<IEnumerable<ModuleTypeDto>> InquireModuleTypesAsync(bool? isActive = null);
    Task<ModuleTypeDto?> GetModuleTypeByIdAsync(int id);
    Task<ModuleTypeDto?> GetModuleTypeByNameAsync(string name);
    Task<ModuleTypeDto> SaveModuleTypeAsync(ModuleTypeUpsertModel model, int? currentUserId);
    Task DeleteModuleTypeAsync(int id);
    #endregion

    #region Module Operations
    Task<IEnumerable<ModuleDto>> InquireModulesAsync(bool? isActive = null, int? moduleTypeId = null, int? parentModuleId = null);
    Task<ModuleDto?> GetModuleByIdAsync(int id);
    Task<ModuleDto?> GetModuleByNameAsync(string name);
    Task<ModuleDto> SaveModuleAsync(ModuleUpsertModel model, int? currentUserId);
    Task DeleteModuleAsync(int id);
    #endregion

    #region Role Permission Operations
    Task<IEnumerable<RoleModulePermissionDto>> GetRolePermissionsAsync(int roleId);
    Task<RoleModulePermissionDto> SaveRolePermissionAsync(RoleModulePermissionUpsertModel model, int? currentUserId);
    Task DeleteRolePermissionAsync(int roleId, int moduleId);
    #endregion

    #region User Permission Operations
    Task<IEnumerable<UserModulePermissionDto>> GetUserPermissionsAsync(int userId);
    Task<UserModulePermissionDto> SaveUserPermissionAsync(UserModulePermissionUpsertModel model, int? currentUserId);
    Task DeleteUserPermissionAsync(int userId, int moduleId);
    #endregion

    #region Menu & Permission Resolution
    Task<IEnumerable<ModuleTypeDto>> GetMenuTreeAsync();
    Task<IEnumerable<ModuleDto>> ResolveUserPermissionsAsync(int userId, int roleId, bool isSuperAdmin = false);
    #endregion
}