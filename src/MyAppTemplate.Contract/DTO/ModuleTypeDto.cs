namespace MyAppTemplate.Contract.DTO;

public class ModuleTypeDto
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

    // Child modules — populated when building the menu tree
    public IEnumerable<ModuleDto> Modules { get; set; } = new List<ModuleDto>();
}