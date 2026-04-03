namespace DamayanFS.App.ViewModels.Permission;

public class PermissionUserViewModel : BaseViewModel
{
    public int UserId { get; set; }
    public string UserFullName { get; set; } = string.Empty;
    public int RoleId { get; set; }
    public string? RoleName { get; set; }
    public bool IsSuperAdmin { get; set; }
}
