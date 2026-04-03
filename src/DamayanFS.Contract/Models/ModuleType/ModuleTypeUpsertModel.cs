using System.ComponentModel.DataAnnotations;

namespace DamayanFS.Contract.Models.ModuleType;

public class ModuleTypeUpsertModel
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [StringLength(100)]
    public string? Icon { get; set; }

    [Display(Name = "Display Order")]
    public int DisplayOrder { get; set; } = 0;

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;

    [Display(Name = "In Maintenance")]
    public bool InMaintenance { get; set; } = false;

    [StringLength(100)]
    public string? Controller { get; set; }

    [StringLength(100)]
    public string? Action { get; set; }
}