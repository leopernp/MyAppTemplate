namespace DamayanFS.Contract.DTO;

public class RoleDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public int? CreatedById { get; set; }
    public DateTime CreatedDate { get; set; }
    public int? ModifiedById { get; set; }
    public DateTime? ModifiedDate { get; set; }

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
}
