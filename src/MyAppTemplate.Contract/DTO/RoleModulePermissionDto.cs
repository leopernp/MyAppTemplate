namespace MyAppTemplate.Contract.DTO;

public class RoleModulePermissionDto
{
    public int Id { get; set; }
    public int RoleId { get; set; }
    public string? RoleName { get; set; }
    public int ModuleId { get; set; }
    public string? ModuleName { get; set; }
    public string? ModuleTypeName { get; set; }

    public bool CanView { get; set; }
    public bool CanCreate { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanDelete { get; set; }

    public int? CreatedById { get; set; }
    public DateTime CreatedDate { get; set; }

    // Audit display
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

    // Computed
    public bool HasAnyPermission => CanView || CanCreate || CanUpdate || CanDelete;
    public bool HasFullPermission => CanView && CanCreate && CanUpdate && CanDelete;
}