using System.ComponentModel.DataAnnotations;
namespace DamayanFS.Data.ContextModels;

public class User
{
    public int Id { get; set; }
    [Required]
    [StringLength(100)]
    public string Username { get; set; } = null!;
    [Required]
    [StringLength(250)]
    public string Password { get; set; } = null!;
    [Required]
    [StringLength(150)]
    public string FirstName { get; set; } = null!;
    [Required]
    [StringLength(150)]
    public string LastName { get; set; } = null!;
    [StringLength(150)]
    public string? MiddleName { get; set; }
    [Required]
    [StringLength(150)]
    public string Email { get; set; } = null!;
    [Required]
    public int RoleId { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsSuperAdmin { get; set; } = false;

    // Lockout tracking
    public int FailedLoginAttempts { get; set; } = 0;
    public DateTime? LockoutEnd { get; set; }

    public int? CreatedById { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public int? ModifiedById { get; set; }
    public DateTime? ModifiedDate { get; set; }

    // Navigation properties
    public Role Role { get; set; } = null!;
    public User? CreatedBy { get; set; }
    public User? ModifiedBy { get; set; }

    public ICollection<UserActivityLog> UserActivityLogs { get; set; } = new List<UserActivityLog>();
}