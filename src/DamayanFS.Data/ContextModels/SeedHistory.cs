namespace MyAppTemplate.Data.ContextModels;

public class SeedHistory
{
    public int Id { get; set; }
    public string Version { get; set; } = string.Empty;
    public DateTime AppliedOn { get; set; }
}
