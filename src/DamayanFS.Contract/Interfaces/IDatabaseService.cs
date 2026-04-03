using DamayanFS.Contract.DTO;

namespace DamayanFS.Contract.Interfaces;

public interface IDatabaseService
{
    Task<IEnumerable<MigrationDto>> GetMigrationHistoryAsync();
    Task<string> GetScriptAsync(string migrationId);
    Task<bool> SyncDatabaseAsync();
    string GetSnapshotInfo();
    Task<bool> RollbackDatabaseAsync(string migrationId);
}
