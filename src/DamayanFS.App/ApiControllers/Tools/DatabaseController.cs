using DamayanFS.Contract.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DamayanFS.App.ApiControllers.Tools
{
    [Route("api/[controller]")]
    [ApiController]
    public class DatabaseController : ControllerBase
    {
        private readonly IDatabaseService _dbService;

        public DatabaseController(IDatabaseService dbService) => _dbService = dbService;

        [HttpGet("migrations")]
        public async Task<IActionResult> GetMigrations()
        {
            var history = await _dbService.GetMigrationHistoryAsync();
            var snapshot = _dbService.GetSnapshotInfo();

            return Ok(new
            {
                SnapshotVersion = snapshot,
                Migrations = history
            });
        }

        [HttpGet("script/{migrationId}")]
        public async Task<IActionResult> GetScript(string migrationId)
        {
            var script = await _dbService.GetScriptAsync(migrationId);
            return Content(script, "text/plain");
        }

        [HttpPost("apply")]
        // [Authorize(Roles = "Admin")] // Important for security
        public async Task<IActionResult> Apply()
        {
            var success = await _dbService.SyncDatabaseAsync();
            if (success) return Ok(new { message = "Database synced successfully." });
            return BadRequest(new { message = "Migration failed. Check server logs." });
        }

        [HttpPost("rollback/{migrationId}")]
        public async Task<IActionResult> Rollback(string migrationId)
        {
            var success = await _dbService.RollbackDatabaseAsync(migrationId);
            if (success) return Ok(new { message = $"Database rolled back to {migrationId}" });
            return BadRequest(new { message = "Rollback failed. This usually happens if data loss would occur." });
        }
    }
}
