using System.Diagnostics;
using MyAppTemplate.App.Models;
using MyAppTemplate.App.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MyAppTemplate.App.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        string pageTitle = "Dashboard";
        var pageHeader = new PageHeaderViewModel()
            .WithTitle(pageTitle, icon: "bi bi-speedometer2")
            .AddBreadcrumb("Home", Url.Action("Index", "Home"))
            .AddBreadcrumb(pageTitle)
            .AddActionButton(
                "Export", 
                "btn btn-sm btn-outline-success", 
                "bi bi-download", 
                buttonType: "button"
            );

        ViewData["Title"] = pageTitle;

        var viewModel = new DashboardViewModel
        {
            PageHeader = pageHeader,

            // Populate other properties of the view model as needed
        };

        return View(viewModel);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
