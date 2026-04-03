using DamayanFS.App.ViewModels.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

using IAuthService = DamayanFS.Contract.Interfaces.IAuthenticationService;

namespace DamayanFS.App.Controllers;

public class AccountController : Controller
{
    private readonly ILogger<AccountController> _logger;
    private readonly IAuthService _authenticationService;

    public AccountController(
        ILogger<AccountController> logger,
        IAuthService authenticationService)
    {
        _logger = logger;
        _authenticationService = authenticationService;
    }

    #region Views

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        // Already authenticated — redirect to home
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");

        var viewModel = new LoginViewModel
        {
            ReturnUrl = returnUrl
        };

        return View(viewModel);
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var browser = Request.Headers.UserAgent.ToString();

        var result = await _authenticationService.AuthenticateAsync(
            model.Username,
            model.Password,
            ipAddress,
            browser);

        if (!result.IsSuccess)
        {
            model.ErrorMessage = result.ErrorMessage;
            model.IsLockedOut = result.IsLockedOut;
            model.RemainingLockoutSeconds = result.RemainingLockoutSeconds;
            return View(model);
        }

        // Build claims principal
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, result.User!.Id.ToString()),
            new(ClaimTypes.Name, result.User.Username),
            new(ClaimTypes.GivenName, result.User.FirstName),
            new(ClaimTypes.Surname, result.User.LastName),
            new(ClaimTypes.Role, result.User.RoleId.ToString()),
            new(ClaimTypes.Email, result.User.Email),
            new("IsSuperAdmin", result.User.IsSuperAdmin.ToString()),
            new("RoleName", result.User.RoleName ?? string.Empty)
        };

        var claimsIdentity = new ClaimsIdentity(
            claims, CookieAuthenticationDefaults.AuthenticationScheme);

        var authProperties = new AuthenticationProperties
        {
            IsPersistent = model.RememberMe,
            ExpiresUtc = model.RememberMe
                ? DateTimeOffset.UtcNow.AddDays(14)
                : null,
            AllowRefresh = true
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

        _logger.LogInformation("User {Username} signed in successfully", result.User.Username);

        // Redirect to return URL if local, otherwise Home
        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            return Redirect(model.ReturnUrl);

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var username = User.FindFirstValue(ClaimTypes.Name) ?? string.Empty;
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var browser = Request.Headers.UserAgent.ToString();

        if (int.TryParse(userId, out var parsedUserId))
        {
            await _authenticationService.LogoutAsync(
                parsedUserId,
                username,
                ipAddress,
                browser);
        }

        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        _logger.LogInformation("User {Username} signed out", username);

        return RedirectToAction("Login", "Account");
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult AccessDenied()
    {
        return View();
    }

    #endregion
}