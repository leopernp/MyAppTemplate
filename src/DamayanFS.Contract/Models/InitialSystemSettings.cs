namespace MyAppTemplate.Contract.Models;

public class InitialSystemSettings
{
    public const string SectionName = "InitialSystemSettings";

    public string DefaultPassword { get; set; } = string.Empty;
    public string AdminEmail { get; set; } = string.Empty;
}
