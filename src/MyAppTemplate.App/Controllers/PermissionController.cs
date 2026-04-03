using MyAppTemplate.App.ViewModels.Permission;
using MyAppTemplate.App.ViewModels.Shared;
using MyAppTemplate.Contract.Interfaces;
using MyAppTemplate.Contract.Models.ModulePermission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MyAppTemplate.App.Controllers;

[Authorize]
[AutoValidateAntiforgeryToken]
public class PermissionController : Controller
{
    private readonly ILogger<PermissionController> _logger;
    private readonly IModuleService _moduleService;
    private readonly ISettingsService _settingsService;

    public PermissionController(
        ILogger<PermissionController> logger,
        IModuleService moduleService,
        ISettingsService settingsService)
    {
        _logger = logger;
        _moduleService = moduleService;
        _settingsService = settingsService;
    }

    #region Views

    public async Task<IActionResult> RolePermissions(int roleId)
    {
        var role = await _settingsService.GetRoleByIdAsync(roleId);
        if (role is null)
            return NotFound();

        string pageTitle = "Permissions";
        var pageHeader = new PageHeaderViewModel()
            .WithTitle(pageTitle, subtitle: role.Name, icon: "bi bi-shield-lock")
            .AddBreadcrumb("Dashboard", Url.Action("Index", "Home"))
            .AddBreadcrumb("Roles", Url.Action("Index", "Role"))
            .AddBreadcrumb(pageTitle)
            .AddActionButton(
                "Back to Roles",
                "btn btn-sm btn-outline-secondary",
                "bi bi-arrow-left",
                url: Url.Action("Index", "Role")
            );

        ViewData["Title"] = $"Permissions — {role.Name}";

        var viewModel = new PermissionRoleViewModel
        {
            PageHeader = pageHeader,
            RoleId = role.Id,
            RoleName = role.Name
        };

        return View(viewModel);
    }

    public async Task<IActionResult> UserPermissions(int userId)
    {
        var user = await _settingsService.GetUserByIdAsync(userId);
        if (user is null)
            return NotFound();

        string pageTitle = "Permissions";
        var pageHeader = new PageHeaderViewModel()
            .WithTitle(pageTitle, subtitle: user.FullName, icon: "bi bi-person-lock")
            .AddBreadcrumb("Dashboard", Url.Action("Index", "Home"))
            .AddBreadcrumb("Users", Url.Action("Index", "User"))
            .AddBreadcrumb(pageTitle)
            .AddActionButton(
                "Back to Users",
                "btn btn-sm btn-outline-secondary",
                "bi bi-arrow-left",
                url: Url.Action("Index", "User")
            );

        ViewData["Title"] = $"Permissions — {user.FullName}";

        var viewModel = new PermissionUserViewModel
        {
            PageHeader = pageHeader,
            UserId = user.Id,
            UserFullName = user.FullName,
            RoleId = user.RoleId,
            RoleName = user.RoleName,
            IsSuperAdmin = user.IsSuperAdmin
        };

        return View(viewModel);
    }

    #endregion

    #region APIs

    [HttpGet]
    public async Task<IActionResult> GetRolePermissionsAsync(int roleId)
    {
        try
        {
            var result = await _moduleService.GetRolePermissionsAsync(roleId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error occurred while getting role permissions.", ex);
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> SaveRolePermissionAsync(RoleModulePermissionUpsertModel model)
    {
        try
        {
            string? userIdClaims = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaims, out var userId))
                throw new InvalidOperationException("User id claims is not available");

            var result = await _moduleService.SaveRolePermissionAsync(model, userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error occurred while saving role permission.", ex);
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteRolePermissionAsync(int roleId, int moduleId)
    {
        try
        {
            await _moduleService.DeleteRolePermissionAsync(roleId, moduleId);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError("Error occurred while deleting role permission.", ex);
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetUserPermissionsAsync(int userId)
    {
        try
        {
            var result = await _moduleService.GetUserPermissionsAsync(userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error occurred while getting user permissions.", ex);
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> SaveUserPermissionAsync(UserModulePermissionUpsertModel model)
    {
        try
        {
            string? userIdClaims = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaims, out var userId))
                throw new InvalidOperationException("User id claims is not available");

            var result = await _moduleService.SaveUserPermissionAsync(model, userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error occurred while saving user permission.", ex);
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteUserPermissionAsync(int userId, int moduleId)
    {
        try
        {
            await _moduleService.DeleteUserPermissionAsync(userId, moduleId);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError("Error occurred while deleting user permission.", ex);
            return BadRequest(ex.Message);
        }
    }

    #endregion
}
