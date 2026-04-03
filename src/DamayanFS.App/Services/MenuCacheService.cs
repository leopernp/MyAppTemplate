using DamayanFS.Contract.DTO;
using DamayanFS.Contract.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace DamayanFS.App.Services;

public class MenuCacheService : IMenuCacheService
{
    private readonly IMemoryCache _cache;
    private readonly IModuleTypeRepository _moduleTypeRepository;
    private readonly IModuleRepository _moduleRepository;
    private readonly IRoleModulePermissionRepository _roleModulePermissionRepository;
    private readonly IUserModulePermissionRepository _userModulePermissionRepository;
    private readonly ILogger<MenuCacheService> _logger;

    private const string CACHE_KEY_PREFIX = "menu_tree_";
    private static readonly TimeSpan CACHE_SLIDING_EXPIRATION = TimeSpan.FromMinutes(30);

    private static readonly HashSet<string> _activeCacheKeys = new();
    private static readonly Lock _lock = new();

    public MenuCacheService(
        IMemoryCache cache,
        IModuleTypeRepository moduleTypeRepository,
        IModuleRepository moduleRepository,
        IRoleModulePermissionRepository roleModulePermissionRepository,
        IUserModulePermissionRepository userModulePermissionRepository,
        ILogger<MenuCacheService> logger)
    {
        _cache = cache;
        _moduleTypeRepository = moduleTypeRepository;
        _moduleRepository = moduleRepository;
        _roleModulePermissionRepository = roleModulePermissionRepository;
        _userModulePermissionRepository = userModulePermissionRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<ModuleTypeDto>> GetMenuAsync(int userId, int roleId, bool isSuperAdmin = false)
    {
        var cacheKey = GetCacheKey(userId);

        if (_cache.TryGetValue(cacheKey, out IEnumerable<ModuleTypeDto>? cached) && cached != null)
            return cached;

        try
        {
            // Load all active modules flat
            var allModules = (await _moduleRepository.InquireAsync(isActive: true)).ToList();

            // Load role and user permission grants
            var rolePermissions = (await _roleModulePermissionRepository.GetRolePermissionsByRoleAsync(roleId))
                .ToDictionary(x => x.ModuleId);
            var userPermissions = (await _userModulePermissionRepository.GetUserPermissionsByUserAsync(userId))
                .ToDictionary(x => x.ModuleId);

            // Load module types for InMaintenance cascade check
            var moduleTypes = (await _moduleTypeRepository.GetMenuTreeAsync()).ToList();
            var moduleTypeLookup = moduleTypes.ToDictionary(x => x.Id);
            var moduleLookup = allModules.ToDictionary(x => x.Id);

            // Resolve permissions and accessibility per module
            foreach (var module in allModules)
            {
                if (isSuperAdmin)
                {
                    module.CanView = module.CanCreate = module.CanUpdate = module.CanDelete = true;
                }
                else
                {
                    rolePermissions.TryGetValue(module.Id, out var rolePerm);
                    userPermissions.TryGetValue(module.Id, out var userPerm);

                    module.CanView   = (rolePerm?.CanView   ?? false) || (userPerm?.CanView   ?? false);
                    module.CanCreate = (rolePerm?.CanCreate ?? false) || (userPerm?.CanCreate ?? false);
                    module.CanUpdate = (rolePerm?.CanUpdate ?? false) || (userPerm?.CanUpdate ?? false);
                    module.CanDelete = (rolePerm?.CanDelete ?? false) || (userPerm?.CanDelete ?? false);
                }

                module.IsAccessible = ResolveAccessibility(module, moduleLookup, moduleTypeLookup);
            }

            // Filter to only modules the user can view — maintenance modules are kept and shown with a badge in the sidebar
            var permittedModules = allModules
                .Where(x => x.CanView)
                .ToList();

            // Attach permitted children to each root module
            var rootModules = permittedModules
                .Where(x => x.ParentModuleId == null)
                .ToList();

            foreach (var module in rootModules)
                AttachPermittedChildren(module, permittedModules);

            // Assign root modules to their ModuleType
            foreach (var moduleType in moduleTypes)
            {
                moduleType.Modules = rootModules
                    .Where(x => x.ModuleTypeId == moduleType.Id)
                    .OrderBy(x => x.DisplayOrder)
                    .ToList();
            }

            // Filter ModuleTypes — keep direct links and section headers with visible children
            var filteredTree = moduleTypes
                .Where(x => x.HasLink || x.Modules.Any())
                .OrderBy(x => x.DisplayOrder)
                .ToList();

            var options = new MemoryCacheEntryOptions
            {
                SlidingExpiration = CACHE_SLIDING_EXPIRATION
            };

            _cache.Set(cacheKey, (IEnumerable<ModuleTypeDto>)filteredTree, options);

            lock (_lock)
                _activeCacheKeys.Add(cacheKey);

            return filteredTree;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while building menu cache for user {UserId}", userId);
            return Enumerable.Empty<ModuleTypeDto>();
        }
    }

    public void InvalidateMenu(int userId)
    {
        var cacheKey = GetCacheKey(userId);
        _cache.Remove(cacheKey);

        lock (_lock)
            _activeCacheKeys.Remove(cacheKey);

        _logger.LogInformation("Menu cache invalidated for user {UserId}", userId);
    }

    public void InvalidateAllMenus()
    {
        lock (_lock)
        {
            foreach (var key in _activeCacheKeys)
                _cache.Remove(key);

            _activeCacheKeys.Clear();
        }

        _logger.LogInformation("Menu cache invalidated for all users");
    }

    private static string GetCacheKey(int userId) => $"{CACHE_KEY_PREFIX}{userId}";

    private void AttachPermittedChildren(ModuleDto parent, List<ModuleDto> permittedModules)
    {
        var children = permittedModules
            .Where(x => x.ParentModuleId == parent.Id)
            .OrderBy(x => x.DisplayOrder)
            .ToList();

        parent.ChildModules = children;

        foreach (var child in children)
            AttachPermittedChildren(child, permittedModules);
    }

    private static bool ResolveAccessibility(
        ModuleDto module,
        Dictionary<int, ModuleDto> moduleLookup,
        Dictionary<int, ModuleTypeDto> moduleTypeLookup)
    {
        if (moduleTypeLookup.TryGetValue(module.ModuleTypeId, out var moduleType))
            if (moduleType.InMaintenance) return false;

        if (module.InMaintenance) return false;

        var visitedIds = new HashSet<int>();
        int? parentId = module.ParentModuleId;

        while (parentId.HasValue)
        {
            if (visitedIds.Contains(parentId.Value)) break;
            visitedIds.Add(parentId.Value);

            if (moduleLookup.TryGetValue(parentId.Value, out var parentModule))
            {
                if (parentModule.InMaintenance) return false;
                parentId = parentModule.ParentModuleId;
            }
            else break;
        }

        return true;
    }
}