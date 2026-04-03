using DamayanFS.App.ViewModels.Shared;
using Microsoft.AspNetCore.Mvc;

namespace DamayanFS.App.ViewComponents;

public class PageHeaderViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(PageHeaderViewModel model)
    {
        return View(model);
    }
}
