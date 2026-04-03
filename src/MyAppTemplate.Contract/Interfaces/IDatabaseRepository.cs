namespace MyAppTemplate.Contract.Interfaces;

public interface IDatabaseRepository
{
    IEnumerable<string> GetMigrations();
    Task<IEnumerable<string>> GetAppliedMigrationsAsync();
    Task MigrateAsync();
    string GetMigrationScript(string migrationId);
    string GetModelSnapshotVersion();
    Task RollbackToAsync(string targetMigrationId);
}
