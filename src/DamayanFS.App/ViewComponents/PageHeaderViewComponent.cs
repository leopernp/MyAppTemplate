using MyAppTemplate.App.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc;

namespace MyAppTemplate.App.ViewComponents;

public class PageHeaderViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(PageHeaderViewModel model)
    {
        return View(model);
    }
}
