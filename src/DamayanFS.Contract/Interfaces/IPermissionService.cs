using MyAppTemplate.Contract.Models.ModulePermission;

namespace MyAppTemplate.Contract.Interfaces;

public interface IPermissionService
{
    Task<ModulePermissions> GetPermissionsAsync(string controller, int userId, int roleId, bool isSuperAdmin);
}
