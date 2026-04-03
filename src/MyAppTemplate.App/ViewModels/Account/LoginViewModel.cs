using System.ComponentModel.DataAnnotations;

namespace MyAppTemplate.App.ViewModels.Account;

public class LoginViewModel
{
    [Required(ErrorMessage = "Username is required.")]
    [Display(Name = "Username")]
    [StringLength(100)]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [Display(Name = "Password")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Remember Me")]
    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }

    // Lockout display — populated by controller on failed login
    public bool IsLockedOut { get; set; }
    public int RemainingLockoutSeconds { get; set; }

    // Error message — populated by controller on failed login
    public string? ErrorMessage { get; set; }

    // Computed — used by view to show lockout countdown vs generic error
    public bool HasError => !string.IsNullOrEmpty(ErrorMessage) || IsLockedOut;
}