namespace DamayanFS.App.ViewModels;

public class DashboardViewModel : BaseViewModel
{
    public string UserName { get; set; }
    public int ActiveProjects { get; set; }
    public int PendingTasks { get; set; }
    public string SystemHealth { get; set; }
}
