using DamayanFS.Contract.DTO;

namespace DamayanFS.App.ViewModels.Shared;

public class SidebarViewModel
{
    public IEnumerable<ModuleTypeDto> MenuTree { get; set; } = new List<ModuleTypeDto>();
    public string CurrentController { get; set; } = string.Empty;
    public string CurrentAction { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Initials { get; set; } = "?";
}
