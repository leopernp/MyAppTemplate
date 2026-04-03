using MyAppTemplate.Contract.DTO;

namespace MyAppTemplate.Contract.Interfaces;

public interface IRoleModulePermissionRepository
{
    Task<RoleModulePermissionDto?> GetRolePermissionAsync(int roleId, int moduleId);
    Task<IEnumerable<RoleModulePermissionDto>> GetRolePermissionsAsync(int roleId);
    Task<IEnumerable<RoleModulePermissionDto>> GetRolePermissionsByRoleAsync(int roleId);
    Task<RoleModulePermissionDto> SaveRolePermissionAsync(RoleModulePermissionDto dto, int? currentUserId);
    Task DeleteRolePermissionAsync(int roleId, int moduleId);
}