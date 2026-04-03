using DamayanFS.Contract.Enums;

namespace DamayanFS.Contract.DTO;

public class UserActivityLogDto
{
    public int Id { get; set; }
    public UserActivityAction Action { get; set; }
    public string? Description { get; set; }
    public int? PerformedById { get; set; }
    public string? PerformedByUsername { get; set; }
    public string? PerformedByFirstName { get; set; }
    public string? PerformedByLastName { get; set; }
    public string? PerformedByDisplayName
    {
        get
        {
            return !string.IsNullOrEmpty(PerformedByFirstName) && !string.IsNullOrEmpty(PerformedByLastName)
                ? $"{PerformedByFirstName} {PerformedByLastName}"
                : PerformedByUsername;
        }
    }
    public string? IpAddress { get; set; }
    public string? Browser { get; set; }
    public DateTime Timestamp { get; set; }
    public string ActionDisplay => Action.ToString();
}
