using MyAppTemplate.Contract.Interfaces;
using MyAppTemplate.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyAppTemplate.Data.Repositories.Tools;

public class DatabaseRepository : IDatabaseRepository
{
    private readonly ApplicationDbContext _context;
    
    public DatabaseRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public IEnumerable<string> GetMigrations()
        => _context.Database.GetMigrations();

    public async Task<IEnumerable<string>> GetAppliedMigrationsAsync()
        => await _context.Database.GetAppliedMigrationsAsync();

    public async Task MigrateAsync() =>
        await _context.Database.MigrateAsync();

    public string GetMigrationScript(string migrationId)
    {
        var migrator = _context.Database.GetService<IMigrator>();
        // Generates script from the "current" state to the specified migration
        return migrator.GenerateScript(fromMigration: null, toMigration: migrationId);
    }

    public string GetModelSnapshotVersion()
    {
        // Access the internal EF Model snapshot metadata
        var model = _context.Model;
        return model.GetAnnotations().FirstOrDefault(a => a.Name.Contains("ProductVersion"))?.Value?.ToString() ?? "Unknown";
    }

    public async Task RollbackToAsync(string targetMigrationId)
    {
        var migrator = _context.Database.GetService<IMigrator>();
        // If targetMigrationId is "0", it reverts ALL migrations
        await migrator.MigrateAsync(targetMigrationId);
    }
}
