using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace DamayanFS.Data.ContextModels;

public class ModuleType
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

    public int? CreatedById { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public int? ModifiedById { get; set; }
    public DateTime? ModifiedDate { get; set; }

    // Navigation properties
    public User? CreatedBy { get; set; }
    public User? ModifiedBy { get; set; }
    public ICollection<Module> Modules { get; set; } = new List<Module>();
}