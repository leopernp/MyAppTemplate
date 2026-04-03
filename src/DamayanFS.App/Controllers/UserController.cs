using DamayanFS.App.ViewModels.Shared;
using DamayanFS.App.ViewModels.User;
using DamayanFS.Contract.Interfaces;
using DamayanFS.Contract.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DamayanFS.App.Controllers;

[Authorize]
[AutoValidateAntiforgeryToken]
public class UserController : Controller
{
    private readonly ILogger<UserController> _logger;
    private readonly ISettingsService _settingsService;
    private readonly IPermissionService _permissionService;

    public UserController(
        ILogger<UserController> logger,
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
        var perms = await _permissionService.GetPermissionsAsync("User", userId, roleId, isSuperAdmin);

        if (!perms.CanView)
            return Forbid();

        string pageTitle = "Users";
        var pageHeader = new PageHeaderViewModel()
            .WithTitle(pageTitle, icon: "bi bi-people")
            .AddBreadcrumb("Dashboard", Url.Action("Index", "Home"))
            .AddBreadcrumb(pageTitle);

        if (perms.CanCreate)
            pageHeader.AddActionButton(
                "Add New User",
                "btn btn-sm btn-outline-primary",
                "bi bi-plus",
                buttonType: "button",
                attributes: new Dictionary<string, string> { ["id"] = "btnAddUser" }
            );

        ViewData["Title"] = pageTitle;

        var viewModel = new UserIndexViewModel()
        {
            PageHeader = pageHeader,
            User = new(),
            CanCreate = perms.CanCreate,
            CanUpdate = perms.CanUpdate,
            CanDelete = perms.CanDelete
        };

        return View(viewModel);
    }

    #endregion

    #region APIs

    [HttpGet]
    public async Task<IActionResult> InquireAsync(int? roleId = null, bool? isActive = null)
    {
        try
        {
            var result = await _settingsService.InquireUsersAsync(isActive, roleId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error occurred while Inquiring Users.", ex);
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        try
        {
            var result = await _settingsService.GetUserByIdAsync(id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error occurred while getting User.", ex.Message);
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> SaveAsync(UserUpsertModel model)
    {
        try
        {
            var (userId, roleId, isSuperAdmin) = ResolveIdentity();
            var perms = await _permissionService.GetPermissionsAsync("User", userId, roleId, isSuperAdmin);

            bool isNew = model.Id == 0;
            if (isNew && !perms.CanCreate) return Forbid();
            if (!isNew && !perms.CanUpdate) return Forbid();

            var result = await _settingsService.SaveUserAsync(model, userId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error occurred while saving User.", ex);
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> ResetPasswordAsync(int id)
    {
        try
        {
            var (userId, roleId, isSuperAdmin) = ResolveIdentity();
            var perms = await _permissionService.GetPermissionsAsync("User", userId, roleId, isSuperAdmin);
            if (!perms.CanUpdate) return Forbid();

            await _settingsService.ResetPasswordAsync(id, userId);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError("Error occurred while resetting password for User.", ex);
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        try
        {
            var (userId, roleId, isSuperAdmin) = ResolveIdentity();
            var perms = await _permissionService.GetPermissionsAsync("User", userId, roleId, isSuperAdmin);
            if (!perms.CanDelete) return Forbid();

            await _settingsService.DeleteUserAsync(id, null, userId);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError("Error occurred while deleting User.", ex);
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> UnlockUserAsync(int id)
    {
        try
        {
            var (userId, roleId, isSuperAdmin) = ResolveIdentity();
            var perms = await _permissionService.GetPermissionsAsync("User", userId, roleId, isSuperAdmin);
            if (!perms.CanUpdate) return Forbid();

            await _settingsService.UnlockUserAsync(id, userId);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError("Error occurred while deleting User.", ex);
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
