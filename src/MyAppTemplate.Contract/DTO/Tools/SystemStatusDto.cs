using System.Text.Json.Serialization;

namespace MyAppTemplate.Contract.DTO.Tools;

public class SystemStatusDto
{
    [JsonPropertyName("Uptime")]
    public string Uptime { get; set; } = "";
    [JsonPropertyName("MemoryUsage")]
    public string MemoryUsage { get; set; } = "";
    [JsonPropertyName("Environment")]
    public string Environment { get; set; } = "";
    [JsonPropertyName("OS")]
    public string OS { get; set; } = "";
    [JsonPropertyName("Framework")]
    public string Framework { get; set; } = "";
    [JsonPropertyName("CpuCount")]
    public int CpuCount { get; set; }
    [JsonPropertyName("ServerTime")]
    public string ServerTime { get; set; } = "";
    [JsonPropertyName("DbStatus")]
    public string DbStatus { get; set; } = "";
}
