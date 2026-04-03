using DamayanFS.App.ViewModels.Shared;
using DamayanFS.Contract.DTO;
using DamayanFS.Contract.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DamayanFS.App.ViewComponents;

public class SidebarViewComponent : ViewComponent
{
    private readonly IMenuCacheService _menuCacheService;
    private readonly ILogger<SidebarViewComponent> _logger;

    public SidebarViewComponent(
        IMenuCacheService menuCacheService,
        ILogger<SidebarViewComponent> logger)
    {
        _menuCacheService = menuCacheService;
        _logger = logger;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        try
        {
            // Resolve current user identity
            var userIdClaim      = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var roleIdClaim      = HttpContext.User.FindFirstValue(ClaimTypes.Role);
            var isSuperAdminClaim = HttpContext.User.FindFirstValue("IsSuperAdmin");

            if (!int.TryParse(userIdClaim, out var userId) ||
                !int.TryParse(roleIdClaim, out var roleId))
            {
                return View(new SidebarViewModel());
            }

            var isSuperAdmin = string.Equals(isSuperAdminClaim, "True", StringComparison.OrdinalIgnoreCase);

            // Resolve current route for active state detection
            var currentController = ViewContext.RouteData.Values["controller"]?.ToString() ?? string.Empty;
            var currentAction = ViewContext.RouteData.Values["action"]?.ToString() ?? string.Empty;

            // Load permitted menu tree from cache
            var menuTree = (await _menuCacheService.GetMenuAsync(userId, roleId, isSuperAdmin)).ToList();

            // Mark active state on modules
            foreach (var moduleType in menuTree)
                MarkActiveState(moduleType, currentController, currentAction);

            var viewModel = new SidebarViewModel
            {
                MenuTree = menuTree,
                CurrentController = currentController,
                CurrentAction = currentAction,
                DisplayName = HttpContext.User.FindFirstValue(ClaimTypes.Name) ?? string.Empty,
                Initials = ResolveInitials(HttpContext.User.FindFirstValue(ClaimTypes.Name))
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while rendering sidebar");
            return View(new SidebarViewModel());
        }
    }

    private void MarkActiveState(ModuleTypeDto moduleType, string currentController, string currentAction)
    {
        // Check if ModuleType itself is the active link
        if (moduleType.HasLink)
        {
            moduleType.IsActive = IsMatch(moduleType.Controller, moduleType.Action, currentController, currentAction);
            return;
        }

        // Check child modules recursively
        foreach (var module in moduleType.Modules)
            MarkModuleActiveState(module, currentController, currentAction);

        // ModuleType is active if any child is active — auto-expand
        moduleType.IsActive = moduleType.Modules.Any(IsActiveRecursive);
    }

    private void MarkModuleActiveState(ModuleDto module, string currentController, string currentAction)
    {
        module.IsActive = IsMatch(module.Controller, module.Action, currentController, currentAction);

        foreach (var child in module.ChildModules)
            MarkModuleActiveState(child, currentController, currentAction);

        // Parent is active if any child is active
        if (!module.IsActive)
            module.IsActive = module.ChildModules.Any(IsActiveRecursive);
    }

    private static bool IsMatch(string? controller, string? action, string currentController, string currentAction)
    {
        return string.Equals(controller, currentController, StringComparison.OrdinalIgnoreCase)
            && string.Equals(action, currentAction, StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsActiveRecursive(ModuleDto module)
    {
        return module.IsActive || module.ChildModules.Any(IsActiveRecursive);
    }

    private static string ResolveInitials(string? displayName)
    {
        if (string.IsNullOrWhiteSpace(displayName)) return "?";
        var parts = displayName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length >= 2
            ? $"{parts[0][0]}{parts[^1][0]}".ToUpper()
            : displayName[..Math.Min(2, displayName.Length)].ToUpper();
    }
}