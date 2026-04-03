using System.ComponentModel.DataAnnotations;

namespace MyAppTemplate.Contract.Models.User;

public class UserUpsertModel
{
    public int Id { get; set; } // 0 for Create, >0 for Edit

    [Required, StringLength(100)]
    public string Username { get; set; } = string.Empty;

    [Required, StringLength(150)]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = string.Empty;

    [Required, StringLength(150)]
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = string.Empty;

    [StringLength(150)]
    [Display(Name = "Middle Name")]
    public string? MiddleName { get; set; }

    [Required, StringLength(150)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Role")]
    public int RoleId { get; set; }

    [Display(Name = "Active")]
    public bool IsActive { get; set; }
}
