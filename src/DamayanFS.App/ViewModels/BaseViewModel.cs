using DamayanFS.App.ViewModels.Shared;

namespace DamayanFS.App.ViewModels;

public class BaseViewModel
{
    public PageHeaderViewModel PageHeader { get; set; } = new();

    // Effective permissions for the current page — populated by each controller's Index action
    public bool CanCreate { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanDelete { get; set; }
}