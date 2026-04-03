using System.ComponentModel.DataAnnotations;

namespace MyAppTemplate.Contract.Models.ModulePermission;

public class UserModulePermissionUpsertModel
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "User")]
    public int UserId { get; set; }

    [Required]
    [Display(Name = "Module")]
    public int ModuleId { get; set; }

    [Display(Name = "View")]
    public bool CanView { get; set; } = false;

    [Display(Name = "Create")]
    public bool CanCreate { get; set; } = false;

    [Display(Name = "Update")]
    public bool CanUpdate { get; set; } = false;

    [Display(Name = "Delete")]
    public bool CanDelete { get; set; } = false;
}