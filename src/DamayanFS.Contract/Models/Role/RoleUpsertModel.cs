using System.ComponentModel.DataAnnotations;

namespace DamayanFS.Contract.Models.Role;

public class RoleUpsertModel
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = null!;

    [StringLength(int.MaxValue)]
    public string? Description { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;
}
