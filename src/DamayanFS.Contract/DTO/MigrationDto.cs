namespace DamayanFS.Contract.DTO;

public class MigrationDto
{
    public string Id { get; set; } = string.Empty;
    public bool IsApplied { get; set; }
    public string Timestamp { get; set; } = string.Empty;
}
