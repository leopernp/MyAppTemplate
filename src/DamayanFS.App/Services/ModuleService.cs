using AutoMapper;
using DamayanFS.Contract.DTO;
using DamayanFS.Contract.Interfaces;
using DamayanFS.Contract.Models.Module;
using DamayanFS.Contract.Models.ModulePermission;
using DamayanFS.Contract.Models.ModuleType;

namespace DamayanFS.App.Services;

public class ModuleService : IModuleService
{
    private readonly ILogger<ModuleService> _logger;
    private readonly IMapper _mapper;

    private readonly IMenuCacheService _menuCacheService;

    private readonly IModuleTypeRepository _moduleTypeRepository;
    private readonly IModuleRepository _moduleRepository;
    private readonly IRoleModulePermissionRepository _roleModulePermissionRepository;
    private readonly IUserModulePermissionRepository _userModulePermissionRepository;

    public ModuleService(
        ILogger<ModuleService> logger,
        IMapper mapper,
        IMenuCacheService menuCacheService,
        IModuleTypeRepository moduleTypeRepository,
        IModuleRepository moduleRepository,
        IRoleModulePermissionRepository roleModulePermissionRepository,
        IUserModulePermissionRepository userModulePermissionRepository)
    {
        _logger = logger;
        _mapper = mapper;

        _menuCacheService = menuCacheService;

        _moduleTypeRepository = moduleTypeRepository;
        _moduleRepository = moduleRepository;
        _roleModulePermissionRepository = roleModulePermissionRepository;
        _userModulePermissionRepository = userModulePermissionRepository;
    }

    #region ModuleType Operations

    public async Task<IEnumerable<ModuleTypeDto>> InquireModuleTypesAsync(bool? isActive = null) =>
        await _moduleTypeRepository.InquireAsync(isActive);

    public async Task<ModuleTypeDto?> GetModuleTypeByIdAsync(int id) =>
        await _moduleTypeRepository.GetByIdAsync(id);

    public async Task<ModuleTypeDto?> GetModuleTypeByNameAsync(string name) =>
        await _moduleTypeRepository.GetByNameAsync(name);

    public async Task<ModuleTypeDto> SaveModuleTypeAsync(ModuleTypeUpsertModel model, int? currentUserId)
    {
        try
        {
            var dto = _mapper.Map<ModuleTypeDto>(model);

            var validation = await _moduleTypeRepository.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                _logger.LogWarning("Validation failed for module type {Name}: {Errors}",
                    model.Name, string.Join(", ", validation.Errors));
                throw new Exception(string.Join("|", validation.Errors));
            }

            ModuleTypeDto result;
            if (dto.Id == 0)
                result = await _moduleTypeRepository.CreateAsync(dto, currentUserId);
            else
                result = await _moduleTypeRepository.UpdateAsync(dto, currentUserId);

            _menuCacheService.InvalidateAllMenus();
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while saving module type {Name}", model.Name);
            throw;
        }
    }

    public async Task DeleteModuleTypeAsync(int id)
    {
        try
        {
            var moduleType = await _moduleTypeRepository.GetByIdAsync(id);
            if (moduleType is null)
                throw new InvalidOperationException($"Module type with ID {id} was not found.");

            var validation = await _moduleTypeRepository.ValidateAsync(moduleType);
            if (!validation.IsValid)
            {
                _logger.LogWarning("Delete validation failed for module type {Id}: {Errors}",
                    id, string.Join(", ", validation.Errors));
                throw new Exception(string.Join("|", validation.Errors));
            }

            await _moduleTypeRepository.DeleteAsync(id);
            _menuCacheService.InvalidateAllMenus();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting module type {Id}", id);
            throw;
        }
    }

    #endregion

    #region Module Operations

    public async Task<IEnumerable<ModuleDto>> InquireModulesAsync(
        bool? isActive = null,
        int? moduleTypeId = null,
        int? parentModuleId = null) =>
        await _moduleRepository.InquireAsync(isActive, moduleTypeId, parentModuleId);

    public async Task<ModuleDto?> GetModuleByIdAsync(int id) =>
        await _moduleRepository.GetByIdAsync(id);

    public async Task<ModuleDto?> GetModuleByNameAsync(string name) =>
        await _moduleRepository.GetByNameAsync(name);

    public async Task<ModuleDto> SaveModuleAsync(ModuleUpsertModel model, int? currentUserId)
    {
        try
        {
            var dto = _mapper.Map<ModuleDto>(model);

            var validation = await _moduleRepository.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                _logger.LogWarning("Validation failed for module {Name}: {Errors}",
                    model.Name, string.Join(", ", validation.Errors));
                throw new Exception(string.Join("|", validation.Errors));
            }

            ModuleDto result;
            if (dto.Id == 0)
                result = await _moduleRepository.CreateAsync(dto, currentUserId);
            else
                result = await _moduleRepository.UpdateAsync(dto, currentUserId);

            _menuCacheService.InvalidateAllMenus();
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while saving module {Name}", model.Name);
            throw;
        }
    }

    public async Task DeleteModuleAsync(int id)
    {
        try
        {
            var module = await _moduleRepository.GetByIdAsync(id);
            if (module is null)
                throw new InvalidOperationException($"Module with ID {id} was not found.");

            var validation = await _moduleRepository.ValidateAsync(module);
            if (!validation.IsValid)
            {
                _logger.LogWarning("Delete validation failed for module {Id}: {Errors}",
                    id, string.Join(", ", validation.Errors));
                throw new Exception(string.Join("|", validation.Errors));
            }

            await _moduleRepository.DeleteAsync(id);
            _menuCacheService.InvalidateAllMenus();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting module {Id}", id);
            throw;
        }
    }

    #endregion

    #region Role Permission Operations

    public async Task<IEnumerable<RoleModulePermissionDto>> GetRolePermissionsAsync(int roleId) =>
        await _roleModulePermissionRepository.GetRolePermissionsAsync(roleId);

    public async Task<RoleModulePermissionDto> SaveRolePermissionAsync(
        RoleModulePermissionUpsertModel model,
        int? currentUserId)
    {
        try
        {
            var dto = _mapper.Map<RoleModulePermissionDto>(model);
            var result = await _roleModulePermissionRepository.SaveRolePermissionAsync(dto, currentUserId);
            _menuCacheService.InvalidateAllMenus();
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while saving role permission. RoleId: {RoleId}, ModuleId: {ModuleId}",
                model.RoleId, model.ModuleId);
            throw;
        }
    }

    public async Task DeleteRolePermissionAsync(int roleId, int moduleId)
    {
        try
        {
            await _roleModulePermissionRepository.DeleteRolePermissionAsync(roleId, moduleId);
            _menuCacheService.InvalidateAllMenus();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting role permission. RoleId: {RoleId}, ModuleId: {ModuleId}",
                roleId, moduleId);
            throw;
        }
    }

    #endregion

    #region User Permission Operations

    public async Task<IEnumerable<UserModulePermissionDto>> GetUserPermissionsAsync(int userId) =>
        await _userModulePermissionRepository.GetUserPermissionsAsync(userId);

    public async Task<UserModulePermissionDto> SaveUserPermissionAsync(
        UserModulePermissionUpsertModel model,
        int? currentUserId)
    {
        try
        {
            var dto = _mapper.Map<UserModulePermissionDto>(model);
            var result = await _userModulePermissionRepository.SaveUserPermissionAsync(dto, currentUserId);
            _menuCacheService.InvalidateMenu(model.UserId);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while saving user permission. UserId: {UserId}, ModuleId: {ModuleId}",
                model.UserId, model.ModuleId);
            throw;
        }
    }

    public async Task DeleteUserPermissionAsync(int userId, int moduleId)
    {
        try
        {
            await _userModulePermissionRepository.DeleteUserPermissionAsync(userId, moduleId);
            _menuCacheService.InvalidateMenu(userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting user permission. UserId: {UserId}, ModuleId: {ModuleId}",
                userId, moduleId);
            throw;
        }
    }

    #endregion

    #region Menu & Permission Resolution

    public async Task<IEnumerable<ModuleTypeDto>> GetMenuTreeAsync()
    {
        try
        {
            // Load flat active module types and all active modules
            var moduleTypes = (await _moduleTypeRepository.GetMenuTreeAsync()).ToList();
            var modules = (await _moduleRepository.InquireAsync(isActive: true)).ToList();

            // Build nested module tree in memory
            var moduleLookup = modules.ToDictionary(x => x.Id);
            var rootModules = modules.Where(x => x.ParentModuleId == null).ToList();

            foreach (var module in rootModules)
                BuildModuleTree(module, modules);

            // Assign root modules to their ModuleType
            foreach (var moduleType in moduleTypes)
            {
                moduleType.Modules = rootModules
                    .Where(x => x.ModuleTypeId == moduleType.Id)
                    .OrderBy(x => x.DisplayOrder)
                    .ToList();
            }

            return moduleTypes;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while building menu tree");
            throw;
        }
    }

    public async Task<IEnumerable<ModuleDto>> ResolveUserPermissionsAsync(int userId, int roleId, bool isSuperAdmin = false)
    {
        try
        {
            // Load all active modules flat
            var modules = (await _moduleRepository.InquireAsync(isActive: true)).ToList();

            // Load module types for InMaintenance cascade check
            var moduleTypes = (await _moduleTypeRepository.InquireAsync()).ToDictionary(x => x.Id);

            // Build module lookup for ancestor chain traversal
            var moduleLookup = modules.ToDictionary(x => x.Id);

            if (isSuperAdmin)
            {
                foreach (var module in modules)
                {
                    module.CanView = module.CanCreate = module.CanUpdate = module.CanDelete = true;
                    module.IsAccessible = ResolveAccessibility(module, moduleLookup, moduleTypes);
                }
            }
            else
            {
                // Load role and user permission grants
                var rolePermissions = (await _roleModulePermissionRepository.GetRolePermissionsByRoleAsync(roleId))
                    .ToDictionary(x => x.ModuleId);
                var userPermissions = (await _userModulePermissionRepository.GetUserPermissionsByUserAsync(userId))
                    .ToDictionary(x => x.ModuleId);

                foreach (var module in modules)
                {
                    // Resolve effective permissions — OR role and user grants
                    rolePermissions.TryGetValue(module.Id, out var rolePerm);
                    userPermissions.TryGetValue(module.Id, out var userPerm);

                    module.CanView   = (rolePerm?.CanView   ?? false) || (userPerm?.CanView   ?? false);
                    module.CanCreate = (rolePerm?.CanCreate ?? false) || (userPerm?.CanCreate ?? false);
                    module.CanUpdate = (rolePerm?.CanUpdate ?? false) || (userPerm?.CanUpdate ?? false);
                    module.CanDelete = (rolePerm?.CanDelete ?? false) || (userPerm?.CanDelete ?? false);

                    module.IsAccessible = ResolveAccessibility(module, moduleLookup, moduleTypes);
                }
            }

            return modules;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while resolving permissions for user {UserId}", userId);
            throw;
        }
    }

    #endregion

    #region Private Helpers

    private void BuildModuleTree(ModuleDto parent, List<ModuleDto> allModules)
    {
        var children = allModules
            .Where(x => x.ParentModuleId == parent.Id)
            .OrderBy(x => x.DisplayOrder)
            .ToList();

        parent.ChildModules = children;

        foreach (var child in children)
            BuildModuleTree(child, allModules);
    }

    private bool ResolveAccessibility(
        ModuleDto module,
        Dictionary<int, ModuleDto> moduleLookup,
        Dictionary<int, ModuleTypeDto> moduleTypeLookup)
    {
        // Check ModuleType InMaintenance
        if (moduleTypeLookup.TryGetValue(module.ModuleTypeId, out var moduleType))
        {
            if (moduleType.InMaintenance)
                return false;
        }

        // Check module's own InMaintenance
        if (module.InMaintenance)
            return false;

        // Walk up the ancestor chain
        var visitedIds = new HashSet<int>();
        int? parentId = module.ParentModuleId;

        while (parentId.HasValue)
        {
            if (visitedIds.Contains(parentId.Value))
                break; // Cycle guard

            visitedIds.Add(parentId.Value);

            if (moduleLookup.TryGetValue(parentId.Value, out var parentModule))
            {
                if (parentModule.InMaintenance)
                    return false;

                parentId = parentModule.ParentModuleId;
            }
            else
            {
                break;
            }
        }

        return true;
    }

    #endregion
}