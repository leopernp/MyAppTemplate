using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace DamayanFS.App.ApiControllers.Tools;

[Route("api/[controller]")]
[ApiController]
public class AppSettingsController : ControllerBase
{
    private readonly IWebHostEnvironment _env;
    private readonly string _filePath;
    private readonly string _backupFolder;

    public AppSettingsController(IWebHostEnvironment env)
    {
        _env = env;
        _filePath = Path.Combine(_env.ContentRootPath, "appsettings.json");
        _backupFolder = Path.Combine(_env.ContentRootPath, "Backups", "Settings");
    }

    [HttpGet]
    public IActionResult GetSettings()
    {
        if (!System.IO.File.Exists(_filePath))
            return NotFound("appsettings.json not found.");
        var jsonContent = System.IO.File.ReadAllText(_filePath);
        return Content(jsonContent, "application/json");
    }

    [HttpPost("save")]
    public async Task<IActionResult> SaveSettings([FromBody] JsonElement newSettings)
    {
        try
        {
            // Ensure backup folder exists
            if (!Directory.Exists(_backupFolder))
                Directory.CreateDirectory(_backupFolder);

            // Create Backup (appsettings.20260402_0900.json.bak)
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string backupPath = Path.Combine(_backupFolder, $"appsettings.{timestamp}.json.bak");
            System.IO.File.Copy(_filePath, backupPath);

            // Save the New Settings with pretty printing
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(newSettings, options);
            await System.IO.File.WriteAllTextAsync(_filePath, jsonString);

            return Ok(new { message = "Settings updated and backup created successfully." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("backups")]
    public IActionResult GetBackups()
    {
        if (!Directory.Exists(_backupFolder)) return Ok(new List<object>());

        var backups = new DirectoryInfo(_backupFolder)
            .GetFiles("*.bak")
            .OrderByDescending(f => f.LastWriteTime)
            .Select(f => new {
                FileName = f.Name,
                Date = f.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss")
            })
            .ToList();

        return Ok(backups);
    }

    [HttpPost("restore/{fileName}")]
    public IActionResult Restore(string fileName)
    {
        var backupPath = Path.Combine(_backupFolder, fileName);
        if (!System.IO.File.Exists(backupPath)) return NotFound("Backup file not found.");

        try
        {
            // Copy backup over the live appsettings.json
            System.IO.File.Copy(backupPath, _filePath, true);
            return Ok(new { message = "Settings restored. Application is restarting..." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
