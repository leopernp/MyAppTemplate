using MyAppTemplate.App.ViewModels.Tools;
using Microsoft.AspNetCore.Mvc;

namespace MyAppTemplate.App.Controllers;

public class ToolsController : Controller
{
    private readonly ILogger<ToolsController> _logger;

    public ToolsController(ILogger<ToolsController> logger)
    {
        _logger = logger;
    }

    #region Views

    public IActionResult Index()
    {
        string pageTitle = "Maintenance Hub";
        var pageHeader = new PageHeaderViewModel()
            .WithTitle(
                pageTitle,
                subtitle: "Centralized administrative and system monitoring tools",
                icon: "bi bi-wrench-adjustable"
            )
            .AddBreadcrumb("Dashboard", Url.Action("Index", "Home"))
            .AddBreadcrumb(pageTitle);

        ViewData["Title"] = pageTitle;

        var viewModel = new ToolIndexViewModel()
        {
            PageHeader = pageHeader
        };

        return View(viewModel);
    }

    public IActionResult LogViewer()
    {
        string pageTitle = "System Log Viewer";
        var pageHeader = new PageHeaderViewModel()
            .WithTitle(
                pageTitle,
                subtitle: "Live monitoring of application trace and error files",
                icon: "bi bi-terminal"
            )
            .AddBreadcrumb("Dashboard", Url.Action("Index", "Home"))
            .AddBreadcrumb("Maintenance Hub", Url.Action("Index", "Tools"))
            .AddBreadcrumb(pageTitle)
            .AddActionButton(
                text: "Back to Hub",
                icon: "bi bi-arrow-left",
                cssClass: "btn btn-sm btn-outline-secondary",
                url: Url.Action("Index", "Tools")
            );

        ViewData["Title"] = pageTitle;

        var viewModel = new ToolLogViewModel()
        {
            PageHeader = pageHeader
        };

        return View(viewModel);
    }

    public IActionResult Settings()
    {
        string pageTitle = "App Settings Editor";
        var pageHeader = new PageHeaderViewModel()
            .WithTitle(
                pageTitle,
                subtitle: "Directly edit appsettings.json with automatic version backups",
                icon: "bi bi-sliders"
            )
            .AddBreadcrumb("Dashboard", Url.Action("Index", "Home"))
            .AddBreadcrumb("Maintenance Hub", Url.Action("Index", "Tools"))
            .AddBreadcrumb(pageTitle)
            .AddActionButton(
                text: "Save Changes",
                icon: "bi bi-save",
                cssClass: "btn btn-sm btn-primary",
                attributes: new Dictionary<string, string>
                {
                    { "id", "btnSaveSettings" },
                    { "onclick", "saveSettings()" }
                }
            );

        ViewData["Title"] = pageTitle;

        var viewModel = new ToolSettingsViewModel()
        {
            PageHeader = pageHeader
        };

        return View(viewModel);
    }

    public IActionResult SystemInfo()
    {
        string pageTitle = "System Information Dashboard";
        var pageHeader = new PageHeaderViewModel()
            .WithTitle(
                pageTitle,
                subtitle: "Real-time insights into server health and performance",
                icon: "bi bi-activity"
            )
            .AddBreadcrumb("Dashboard", Url.Action("Index", "Home"))
            .AddBreadcrumb("Maintenance Hub", Url.Action("Index", "Tools"))
            .AddBreadcrumb(pageTitle)
            .AddActionButton(
                text: "Refresh Data",
                icon: "bi bi-arrow-clockwise",
                cssClass: "btn btn-sm btn-outline-primary",
                attributes: new Dictionary<string, string>
                {
                    { "id", "btnRefreshInfo" },
                    { "onclick", "refreshSystemInfo()" }
                }
            );

        ViewData["Title"] = pageTitle;

        var viewModel = new ToolSystemInfoViewModel()
        {
            PageHeader = pageHeader
        };
        return View(viewModel);
    }

    public IActionResult Migrations()
    {
        string pageTitle = "Database Migrations";
        var pageHeader = new PageHeaderViewModel()
            .WithTitle(
                pageTitle,
                subtitle: "Track Entity Framework schema changes and sync status",
                icon: "bi bi-layers-half"
            )
            .AddBreadcrumb("Dashboard", Url.Action("Index", "Home"))
            .AddBreadcrumb("Maintenance Hub", Url.Action("Index", "Tools"))
            .AddBreadcrumb(pageTitle)
            .AddActionButton(
                text: "Sync Database",
                icon: "bi bi-lightning-charge-fill",
                cssClass: "btn btn-sm btn-warning d-none",
                attributes: new Dictionary<string, string>
                {
                    { "id", "btn-sync-db" },
                    { "onclick", "syncDatabase()" }
                }
            )
            .AddActionButton(
                text: "Refresh List",
                icon: "bi bi-arrow-clockwise",
                cssClass: "btn btn-sm btn-outline-primary",
                attributes: new Dictionary<string, string>
                {
                    { "id", "btnLoadMigrations" },
                    { "onclick", "loadMigrations()" }
                }
            );

        ViewData["Title"] = pageTitle;

        var viewModel = new ToolMigrationsViewModel()
        {
            PageHeader = pageHeader
        };
        return View(viewModel);
    }

    public IActionResult CacheManager()
    {
        string pageTitle = "Cache Management";

        var pageHeader = new PageHeaderViewModel()
            .WithTitle(
                pageTitle,
                subtitle: "Monitor and flush system-wide In-Memory or Distributed cache entries",
                icon: "bi bi-database-fill-gear" // Appropriate Bootstrap icon
            )
            .AddBreadcrumb("Dashboard", Url.Action("Index", "Home"))
            .AddBreadcrumb("Maintenance Hub", Url.Action("Index", "Tools"))
            .AddBreadcrumb(pageTitle)
            .AddActionButton(
                text: "Flush All Cache",
                icon: "bi bi-trash3-fill",
                cssClass: "btn btn-sm btn-danger",
                attributes: new Dictionary<string, string>
                {
                { "id", "btn-flush-cache" },
                { "onclick", "confirmFlushAll()" } // Handled via JS in your view
                }
            )
            .AddActionButton(
                text: "Refresh Entries",
                icon: "bi bi-arrow-clockwise",
                cssClass: "btn btn-sm btn-outline-primary",
                attributes: new Dictionary<string, string>
                {
                { "id", "btnLoadCache" },
                { "onclick", "loadCacheEntries()" }
                }
            );

        ViewData["Title"] = pageTitle;

        // Assuming a similar ViewModel structure to your Migrations tool
        var viewModel = new ToolCacheViewModel()
        {
            PageHeader = pageHeader
        };

        return View(viewModel);
    }

    #endregion
}
