using MyAppTemplate.App.ViewModels.Role;
using MyAppTemplate.App.ViewModels.Shared;
using MyAppTemplate.Contract.Interfaces;
using MyAppTemplate.Contract.Models.Role;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MyAppTemplate.App.Controllers;

[Authorize]
[AutoValidateAntiforgeryToken]
public class RoleController : Controller
{
    private readonly ILogger<RoleController> _logger;
    private readonly ISettingsService _settingsService;
    private readonly IPermissionService _permissionService;

    public RoleController(
        ILogger<RoleController> logger,
        ISettingsService settingsService,
        IPermissionService permissionService)
    {
        _logger = logger;
        _settingsService = settingsService;
        _permissionService = permissionService;
    }

    #region Views

    public async Task<IActionResult> Index()
    {
        var (userId, roleId, isSuperAdmin) = ResolveIdentity();
        var perms = await _permissionService.GetPermissionsAsync("Role", userId, roleId, isSuperAdmin);

        if (!perms.CanView)
            return Forbid();

        string pageTitle = "Roles";
        var pageHeader = new PageHeaderViewModel()
            .WithTitle(pageTitle, icon: "bi bi-shield-lock")
            .AddBreadcrumb("Dashboard", Url.Action("Index", "Home"))
            .AddBreadcrumb(pageTitle);

        if (perms.CanCreate)
            pageHeader.AddActionButton(
                "Add New Role",
                "btn btn-sm btn-outline-primary",
                "bi bi-plus",
                buttonType: "button",
                attributes: new Dictionary<string, string> { ["id"] = "btnAddRole" }
            );

        ViewData["Title"] = pageTitle;

        var viewModel = new RoleIndexViewModel()
        {
            PageHeader = pageHeader,
            Role = new(),
            CanCreate = perms.CanCreate,
            CanUpdate = perms.CanUpdate,
            CanDelete = perms.CanDelete
        };

        return View(viewModel);
    }

    #endregion

    #region APIs

    [HttpGet]
    public async Task<IActionResult> InquireAsync(bool? isActive = null)
    {
        try
        {
            var result = await _settingsService.InquireRolesAsync(isActive);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error occurred while inquiring Roles.", ex);
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        try
        {
            var result = await _settingsService.GetRoleByIdAsync(id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error occurred while getting Role.", ex);
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> SaveAsync(RoleUpsertModel model)
    {
        try
        {
            var (userId, roleId, isSuperAdmin) = ResolveIdentity();
            var perms = await _permissionService.GetPermissionsAsync("Role", userId, roleId, isSuperAdmin);

            bool isNew = model.Id == 0;
            if (isNew && !perms.CanCreate) return Forbid();
            if (!isNew && !perms.CanUpdate) return Forbid();

            var result = await _settingsService.SaveRoleAsync(model, userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error occurred while saving Role.", ex);
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        try
        {
            var (userId, roleId, isSuperAdmin) = ResolveIdentity();
            var perms = await _permissionService.GetPermissionsAsync("Role", userId, roleId, isSuperAdmin);
            if (!perms.CanDelete) return Forbid();

            await _settingsService.DeleteRoleAsync(id);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError("Error occurred while deleting Role.", ex);
            return BadRequest(ex.Message);
        }
    }

    #endregion

    private (int userId, int roleId, bool isSuperAdmin) ResolveIdentity()
    {
        int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId);
        int.TryParse(User.FindFirstValue(ClaimTypes.Role), out var roleId);
        var isSuperAdmin = string.Equals(User.FindFirstValue("IsSuperAdmin"), "True", StringComparison.OrdinalIgnoreCase);
        return (userId, roleId, isSuperAdmin);
    }
}
