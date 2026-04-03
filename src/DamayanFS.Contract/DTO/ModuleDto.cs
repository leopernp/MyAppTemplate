namespace DamayanFS.Contract.DTO;

public class ModuleDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public bool InMaintenance { get; set; }
    public string? Controller { get; set; }
    public string? Action { get; set; }
    public int ModuleTypeId { get; set; }
    public string? ModuleTypeName { get; set; }
    public int? ParentModuleId { get; set; }
    public string? ParentModuleName { get; set; }

    public int? CreatedById { get; set; }
    public DateTime CreatedDate { get; set; }
    public int? ModifiedById { get; set; }
    public DateTime? ModifiedDate { get; set; }

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

    // Computed
    public bool HasLink => !string.IsNullOrEmpty(Controller) && !string.IsNullOrEmpty(Action);
    public bool IsGroupNode => !HasLink;

    // Tree structure — populated when building the menu tree
    public IEnumerable<ModuleDto> ChildModules { get; set; } = new List<ModuleDto>();

    // Effective permissions — populated during permission resolution
    public bool CanView { get; set; }
    public bool CanCreate { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanDelete { get; set; }

    // Accessibility — resolved at runtime
    public bool IsAccessible { get; set; } = true;
}