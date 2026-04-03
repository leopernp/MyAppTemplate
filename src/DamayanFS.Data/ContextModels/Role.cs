using System.ComponentModel.DataAnnotations;

namespace MyAppTemplate.Data.ContextModels;

public class Role
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    [StringLength(int.MaxValue)]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public int? CreatedById { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public int? ModifiedById { get; set; }

    public DateTime? ModifiedDate { get; set; }

    // Navigation property
    public ICollection<User> Users { get; set; } = new List<User>();

    public User? CreatedBy { get; set; }
    public User? ModifiedBy { get; set; }
}
