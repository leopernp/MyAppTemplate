using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MyAppTemplate.App.ApiControllers.Tools;

[Route("api/[controller]")]
[ApiController]
public class LogViewerController : ControllerBase
{
    private readonly string _logDirectory;

    public LogViewerController(IWebHostEnvironment env)
    {
        //Points to a 'Logs' folder in the content root of the application
        _logDirectory = Path.Combine(env.ContentRootPath, "Logs");
    }

    [HttpGet("files")]
    public IActionResult GetLogFiles()
    {
        if (!Directory.Exists(_logDirectory))
            return NotFound("Log directory not found.");

        var directoryInfo = new DirectoryInfo(_logDirectory);

        var logFiles = directoryInfo.GetFiles()
            .OrderByDescending(f => f.LastWriteTime)
            .Select(f => new {
                FileName = f.Name,
                LastModified = f.LastWriteTime.ToString("yyyy-MM-dd HH:mm")
            })
            .ToList();

        return Ok(logFiles);
    }

    [HttpGet("view/{fileName}")]
    public IActionResult ViewLogFile(string fileName)
    {
        var filePath = Path.Combine(_logDirectory, fileName);

        // Security check to prevent path traversal attacks
        if (!System.IO.File.Exists(filePath) || !filePath.StartsWith(_logDirectory))
            return NotFound("Log file not found.");

        // Use FileShare.ReadWrite to allow reading while the file is being written to
        using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var reader = new StreamReader(stream);
        var logContent = reader.ReadToEnd();

        return Content(logContent, "text/plain");
    }
}
