using DamayanFS.App.ViewModels.ModuleType;
using DamayanFS.App.ViewModels.Shared;
using DamayanFS.Contract.Interfaces;
using DamayanFS.Contract.Models.ModuleType;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DamayanFS.App.Controllers;

[Authorize]
[AutoValidateAntiforgeryToken]
public class ModuleTypeController : Controller
{
    private readonly ILogger<ModuleTypeController> _logger;
    private readonly ISettingsService _settingsService;
    private readonly IPermissionService _permissionService;

    public ModuleTypeController(
        ILogger<ModuleTypeController> logger,
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
        var perms = await _permissionService.GetPermissionsAsync("ModuleType", userId, roleId, isSuperAdmin);

        if (!perms.CanView)
            return Forbid();

        string pageTitle = "Module Types";
        var pageHeader = new PageHeaderViewModel()
            .WithTitle(pageTitle, icon: "bi bi-collection")
            .AddBreadcrumb("Dashboard", Url.Action("Index", "Home"))
            .AddBreadcrumb(pageTitle);

        if (perms.CanCreate)
            pageHeader.AddActionButton(
                "Add New Module Type",
                "btn btn-sm btn-outline-primary",
                "bi bi-plus",
                buttonType: "button",
                attributes: new Dictionary<string, string> { ["id"] = "btnAddModuleType" }
            );

        ViewData["Title"] = pageTitle;

        var viewModel = new ModuleTypeIndexViewModel()
        {
            PageHeader = pageHeader,
            ModuleType = new(),
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
            var result = await _settingsService.InquireModuleTypesAsync(isActive);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while inquiring Module Types.");
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        try
        {
            var result = await _settingsService.GetModuleTypeByIdAsync(id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting Module Type.");
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> SaveAsync(ModuleTypeUpsertModel model)
    {
        try
        {
            var (userId, roleId, isSuperAdmin) = ResolveIdentity();
            var perms = await _permissionService.GetPermissionsAsync("ModuleType", userId, roleId, isSuperAdmin);

            bool isNew = model.Id == 0;
            if (isNew && !perms.CanCreate) return Forbid();
            if (!isNew && !perms.CanUpdate) return Forbid();

            var result = await _settingsService.SaveModuleTypeAsync(model, userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while saving Module Type.");
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        try
        {
            var (userId, roleId, isSuperAdmin) = ResolveIdentity();
            var perms = await _permissionService.GetPermissionsAsync("ModuleType", userId, roleId, isSuperAdmin);
            if (!perms.CanDelete) return Forbid();

            await _settingsService.DeleteModuleTypeAsync(id);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting Module Type.");
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
