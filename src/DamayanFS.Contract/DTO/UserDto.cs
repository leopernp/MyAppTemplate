namespace MyAppTemplate.Contract.DTO;

public class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? MiddleName { get; set; }
    public string Email { get; set; } = null!;
    public int RoleId { get; set; }
    public bool IsActive { get; set; }
    public bool IsSuperAdmin { get; set; }

    // Lockout tracking
    public int FailedLoginAttempts { get; set; }
    public DateTime? LockoutEnd { get; set; }

    // Computed lockout properties
    public bool IsLockedOut => LockoutEnd.HasValue && LockoutEnd.Value > DateTime.UtcNow;
    public int RemainingLockoutSeconds => IsLockedOut
        ? (int)Math.Ceiling((LockoutEnd!.Value - DateTime.UtcNow).TotalSeconds)
        : 0;

    public int? CreatedById { get; set; }
    public DateTime CreatedDate { get; set; }
    public int? ModifiedById { get; set; }
    public DateTime? ModifiedDate { get; set; }
    // Computed property for display
    public string FullName => $"{FirstName} {LastName}".Trim();
    // Audit display properties
    public string? CreatedByUsername { get; set; }
    public string? CreatedByFirstName { get; set; }
    public string? CreatedByLastName { get; set; }
    public string? CreatedByDisplayName
    {
        get
        {
            return !string.IsNullOrEmpty(CreatedByFirstName) && !string.IsNullOrEmpty(CreatedByLastName)
                ? $"{CreatedByFirstName} {CreatedByLastName}"
                : CreatedByUsername;
        }
    }
    public string? ModifiedByUsername { get; set; }
    public string? ModifiedByFirstName { get; set; }
    public string? ModifiedByLastName { get; set; }
    public string? ModifiedByDisplayName
    {
        get
        {
            return !string.IsNullOrEmpty(ModifiedByFirstName) && !string.IsNullOrEmpty(ModifiedByLastName)
                ? $"{ModifiedByFirstName} {ModifiedByLastName}"
                : ModifiedByUsername;
        }
    }
    public string? RoleName { get; set; }
}