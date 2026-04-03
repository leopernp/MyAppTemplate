using DamayanFS.Contract.Models.ModulePermission;

namespace DamayanFS.Contract.Interfaces;

public interface IPermissionService
{
    Task<ModulePermissions> GetPermissionsAsync(string controller, int userId, int roleId, bool isSuperAdmin);
}
