namespace MyAppTemplate.Data.ContextModels;

public class RoleModulePermission
{
    public int Id { get; set; }

    public int RoleId { get; set; }
    public int ModuleId { get; set; }

    public bool CanView { get; set; } = false;
    public bool CanCreate { get; set; } = false;
    public bool CanUpdate { get; set; } = false;
    public bool CanDelete { get; set; } = false;

    public int? CreatedById { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Role Role { get; set; } = null!;
    public Module Module { get; set; } = null!;
    public User? CreatedBy { get; set; }
}