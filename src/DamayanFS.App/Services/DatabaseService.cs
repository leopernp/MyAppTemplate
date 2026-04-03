using DamayanFS.Contract.DTO;
using DamayanFS.Contract.Interfaces;

namespace DamayanFS.App.Services;

public class DatabaseService : IDatabaseService
{
    private readonly IDatabaseRepository _dbRepo;

    public DatabaseService(IDatabaseRepository dbRepo) => _dbRepo = dbRepo;

    public async Task<IEnumerable<MigrationDto>> GetMigrationHistoryAsync()
    {
        var all = _dbRepo.GetMigrations();
        var applied = await _dbRepo.GetAppliedMigrationsAsync();

        return all.Select(id => new MigrationDto
        {
            Id = id,
            IsApplied = applied.Contains(id),
            Timestamp = ExtractDate(id)
        }).OrderByDescending(x => x.Id);
    }

    public async Task<string> GetScriptAsync(string migrationId)
    {
        return _dbRepo.GetMigrationScript(migrationId);
    }

    public async Task<bool> SyncDatabaseAsync()
    {
        try
        {
            await _dbRepo.MigrateAsync();
            return true;
        }
        catch { return false; }
    }

    public string GetSnapshotInfo() => _dbRepo.GetModelSnapshotVersion();

    private string ExtractDate(string id)
    {
        if (id.Length >= 14 && long.TryParse(id.Substring(0, 14), out _))
        {
            var ts = id.Substring(0, 14);
            return $"{ts.Substring(0, 4)}-{ts.Substring(4, 2)}-{ts.Substring(6, 2)} {ts.Substring(8, 2)}:{ts.Substring(10, 2)}";
        }
        return "Unknown";
    }

    public async Task<bool> RollbackDatabaseAsync(string migrationId)
    {
        try
        {
            await _dbRepo.RollbackToAsync(migrationId);
            return true;
        }
        catch { return false; }
    }
}
