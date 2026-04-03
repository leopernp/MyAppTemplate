using DamayanFS.Contract.Interfaces;
using DamayanFS.Contract.Models.ModulePermission;

namespace DamayanFS.App.Services;

public class PermissionService : IPermissionService
{
    private readonly IModuleRepository _moduleRepository;
    private readonly IRoleModulePermissionRepository _roleModulePermissionRepository;
    private readonly IUserModulePermissionRepository _userModulePermissionRepository;
    private readonly ILogger<PermissionService> _logger;

    public PermissionService(
        IModuleRepository moduleRepository,
        IRoleModulePermissionRepository roleModulePermissionRepository,
        IUserModulePermissionRepository userModulePermissionRepository,
        ILogger<PermissionService> logger)
    {
        _moduleRepository = moduleRepository;
        _roleModulePermissionRepository = roleModulePermissionRepository;
        _userModulePermissionRepository = userModulePermissionRepository;
        _logger = logger;
    }

    public async Task<ModulePermissions> GetPermissionsAsync(
        string controller, int userId, int roleId, bool isSuperAdmin)
    {
        if (isSuperAdmin)
            return ModulePermissions.FullAccess;

        try
        {
            var modules = await _moduleRepository.InquireAsync(isActive: true);
            var module = modules.FirstOrDefault(m =>
                string.Equals(m.Controller, controller, StringComparison.OrdinalIgnoreCase));

            if (module is null)
                return ModulePermissions.NoAccess;

            var rolePerm = await _roleModulePermissionRepository.GetRolePermissionAsync(roleId, module.Id);
            var userPerm = await _userModulePermissionRepository.GetUserPermissionAsync(userId, module.Id);

            return new ModulePermissions(
                CanView:   (rolePerm?.CanView   ?? false) || (userPerm?.CanView   ?? false),
                CanCreate: (rolePerm?.CanCreate ?? false) || (userPerm?.CanCreate ?? false),
                CanUpdate: (rolePerm?.CanUpdate ?? false) || (userPerm?.CanUpdate ?? false),
                CanDelete: (rolePerm?.CanDelete ?? false) || (userPerm?.CanDelete ?? false)
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resolving permissions for controller {Controller}", controller);
            return ModulePermissions.NoAccess;
        }
    }
}
