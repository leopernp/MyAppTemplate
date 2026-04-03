using System.ComponentModel.DataAnnotations;

namespace DamayanFS.Data.ContextModels;

public class Module
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    [StringLength(500)]
    public string? Description { get; set; }

    [StringLength(100)]
    public string? Icon { get; set; }

    public int DisplayOrder { get; set; } = 0;

    public bool IsActive { get; set; } = true;

    public bool InMaintenance { get; set; } = false;

    [StringLength(100)]
    public string? Controller { get; set; }

    [StringLength(100)]
    public string? Action { get; set; }

    [Required]
    public int ModuleTypeId { get; set; }

    public int? ParentModuleId { get; set; }

    public int? CreatedById { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public int? ModifiedById { get; set; }
    public DateTime? ModifiedDate { get; set; }

    // Navigation properties
    public ModuleType ModuleType { get; set; } = null!;
    public Module? ParentModule { get; set; }
    public ICollection<Module> ChildModules { get; set; } = new List<Module>();
    public User? CreatedBy { get; set; }
    public User? ModifiedBy { get; set; }
    public ICollection<RoleModulePermission> RolePermissions { get; set; } = new List<RoleModulePermission>();
    public ICollection<UserModulePermission> UserPermissions { get; set; } = new List<UserModulePermission>();
}