using DamayanFS.App.ViewModels.Module;
using DamayanFS.App.ViewModels.Shared;
using DamayanFS.Contract.Interfaces;
using DamayanFS.Contract.Models.Module;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DamayanFS.App.Controllers;

[Authorize]
[AutoValidateAntiforgeryToken]
public class ModuleController : Controller
{
    private readonly ILogger<ModuleController> _logger;
    private readonly IModuleService _moduleService;
    private readonly IPermissionService _permissionService;

    public ModuleController(
        ILogger<ModuleController> logger,
        IModuleService moduleService,
        IPermissionService permissionService)
    {
        _logger = logger;
        _moduleService = moduleService;
        _permissionService = permissionService;
    }

    #region Views

    public async Task<IActionResult> Index()
    {
        var (userId, roleId, isSuperAdmin) = ResolveIdentity();
        var perms = await _permissionService.GetPermissionsAsync("Module", userId, roleId, isSuperAdmin);

        if (!perms.CanView)
            return Forbid();

        string pageTitle = "Modules";
        var pageHeader = new PageHeaderViewModel()
            .WithTitle(pageTitle, icon: "bi bi-diagram-3")
            .AddBreadcrumb("Dashboard", Url.Action("Index", "Home"))
            .AddBreadcrumb(pageTitle);

        if (perms.CanCreate)
            pageHeader.AddActionButton(
                "Add New Module",
                "btn btn-sm btn-outline-primary",
                "bi bi-plus",
                buttonType: "button",
                attributes: new Dictionary<string, string> { ["id"] = "btnAddModule" }
            );

        ViewData["Title"] = pageTitle;

        var viewModel = new ModuleIndexViewModel()
        {
            PageHeader = pageHeader,
            Module = new(),
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
            var result = await _moduleService.InquireModulesAsync(isActive);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while inquiring Modules.");
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        try
        {
            var result = await _moduleService.GetModuleByIdAsync(id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting Module.");
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> SaveAsync(ModuleUpsertModel model)
    {
        try
        {
            var (userId, roleId, isSuperAdmin) = ResolveIdentity();
            var perms = await _permissionService.GetPermissionsAsync("Module", userId, roleId, isSuperAdmin);

            bool isNew = model.Id == 0;
            if (isNew && !perms.CanCreate) return Forbid();
            if (!isNew && !perms.CanUpdate) return Forbid();

            var result = await _moduleService.SaveModuleAsync(model, userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while saving Module.");
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        try
        {
            var (userId, roleId, isSuperAdmin) = ResolveIdentity();
            var perms = await _permissionService.GetPermissionsAsync("Module", userId, roleId, isSuperAdmin);
            if (!perms.CanDelete) return Forbid();

            await _moduleService.DeleteModuleAsync(id);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting Module.");
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAvailableParentModulesAsync(int? excludeId = null, int? moduleTypeId = null)
    {
        try
        {
            var modules = await _moduleService.InquireModulesAsync(isActive: true);
            var excludeIds = new HashSet<int>();

            // Exclude the current module
            if (excludeId.HasValue)
            {
                excludeIds.Add(excludeId.Value);

                // Recursively get all descendants of the current module to prevent circular references
                GetDescendantIds(excludeId.Value, modules, excludeIds);
            }

            var available = modules
                .Where(x => !excludeIds.Contains(x.Id))
                .Where(x => !moduleTypeId.HasValue || x.ModuleTypeId == moduleTypeId.Value)
                .ToList();

            return Ok(available);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting available parent modules.");
            return BadRequest(ex.Message);
        }
    }

    private void GetDescendantIds(int parentId, IEnumerable<dynamic> modules, HashSet<int> excludeIds)
    {
        var children = modules.Where(x => x.ParentModuleId == parentId).ToList();
        foreach (var child in children)
        {
            excludeIds.Add(child.Id);
            GetDescendantIds(child.Id, modules, excludeIds);
        }
    }

    private (int userId, int roleId, bool isSuperAdmin) ResolveIdentity()
    {
        int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId);
        int.TryParse(User.FindFirstValue(ClaimTypes.Role), out var roleId);
        var isSuperAdmin = string.Equals(User.FindFirstValue("IsSuperAdmin"), "True", StringComparison.OrdinalIgnoreCase);
        return (userId, roleId, isSuperAdmin);
    }

    #endregion
}
