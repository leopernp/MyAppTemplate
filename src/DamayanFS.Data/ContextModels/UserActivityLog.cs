using DamayanFS.Contract.Enums;
using System.ComponentModel.DataAnnotations;

namespace DamayanFS.Data.ContextModels;

public class UserActivityLog
{
    public int Id { get; set; }

    [Required]
    public UserActivityAction Action { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    public int? PerformedById { get; set; }

    [StringLength(45)]
    public string? IpAddress { get; set; }

    [StringLength(500)]
    public string? Browser { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    // Navigation property
    public User? PerformedBy { get; set; }
}
