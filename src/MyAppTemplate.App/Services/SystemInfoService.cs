using MyAppTemplate.Contract.DTO.Tools;
using MyAppTemplate.Contract.Interfaces;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MyAppTemplate.App.Services;

public class SystemInfoService : ISystemInfoService
{
    private readonly IConfiguration _config;
    private readonly IWebHostEnvironment _env;

    public SystemInfoService(IConfiguration config, IWebHostEnvironment env)
    {
        _config = config;
        _env = env;
    }

    public async Task<SystemStatusDto> GetSystemMetricsAsync()
    {
        var process = Process.GetCurrentProcess();
        return new SystemStatusDto()
        {
            Environment = _env.EnvironmentName,
            OS = RuntimeInformation.OSDescription,
            Framework = RuntimeInformation.FrameworkDescription,
            Uptime = (DateTime.Now - process.StartTime).ToString(@"dd\.hh\:mm\:ss"),
            MemoryUsage = $"{process.PrivateMemorySize64 / 1024 / 1024} MB",
            CpuCount = Environment.ProcessorCount,
            ServerTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            DbStatus = await CheckDatabaseConnection()
        };
    }

    private async Task<string> CheckDatabaseConnection()
    {
        try
        {
            var connectionString = _config.GetConnectionString("Default");
            using var conn = new SqlConnection(connectionString);
            await conn.OpenAsync();
            return "Healthy";
        }
        catch { return "Offline"; }
    }
}
