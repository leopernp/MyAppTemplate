using DamayanFS.Data.ContextModels;
using Microsoft.EntityFrameworkCore;

namespace DamayanFS.Data.Context;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<SeedHistory> SeedHistories => Set<SeedHistory>();

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserActivityLog> UserActivityLogs { get; set; }

    public DbSet<ModuleType> ModuleTypes { get; set; }
    public DbSet<Module> Modules { get; set; }
    public DbSet<RoleModulePermission> RoleModulePermissions { get; set; }
    public DbSet<UserModulePermission> UserModulePermissions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Iterate through all entities and remove pluralization
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            // This sets the table name to the name of the C# class
            entity.SetTableName(entity.DisplayName());
        }

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId);

            entity.HasOne(u => u.CreatedBy)
                .WithMany()
                .HasForeignKey(u => u.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(u => u.ModifiedBy)
                .WithMany()
                .HasForeignKey(u => u.ModifiedById)
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(u => u.IsActive)
                .HasDefaultValue(true);

            entity.Property(u => u.IsSuperAdmin)
                .HasDefaultValue(false);

            entity.Property(u => u.CreatedDate)
                .HasDefaultValueSql("GETUTCDATE()");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasOne(r => r.CreatedBy)
               .WithMany()
               .HasForeignKey(r => r.CreatedById)
               .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(r => r.ModifiedBy)
                .WithMany()
                .HasForeignKey(r => r.ModifiedById)
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(r => r.IsActive)
                .HasDefaultValue(true);
        });

        modelBuilder.Entity<UserActivityLog>(entity =>
        {
            entity.Property(x => x.Action)
                .HasConversion<string>()
                .HasMaxLength(50);

            entity.HasOne(x => x.PerformedBy)
                .WithMany(u => u.UserActivityLogs)
                .HasForeignKey(x => x.PerformedById)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<ModuleType>(entity =>
        {
            entity.HasOne(x => x.CreatedBy)
                .WithMany()
                .HasForeignKey(x => x.CreatedById)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(x => x.ModifiedBy)
                .WithMany()
                .HasForeignKey(x => x.ModifiedById)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<Module>(entity =>
        {
            entity.HasOne(x => x.ModuleType)
                .WithMany(x => x.Modules)
                .HasForeignKey(x => x.ModuleTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.ParentModule)
                .WithMany(x => x.ChildModules)
                .HasForeignKey(x => x.ParentModuleId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.CreatedBy)
                .WithMany()
                .HasForeignKey(x => x.CreatedById)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(x => x.ModifiedBy)
                .WithMany()
                .HasForeignKey(x => x.ModifiedById)
                .OnDelete(DeleteBehavior.NoAction);
        });


        modelBuilder.Entity<RoleModulePermission>(entity =>
        {
            entity.HasOne(x => x.Role)
                .WithMany()
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Module)
                .WithMany(x => x.RolePermissions)
                .HasForeignKey(x => x.ModuleId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.CreatedBy)
                .WithMany()
                .HasForeignKey(x => x.CreatedById)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasIndex(x => new { x.RoleId, x.ModuleId })
                .IsUnique();
        });

        modelBuilder.Entity<UserModulePermission>(entity =>
        {
            entity.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Module)
                .WithMany(x => x.UserPermissions)
                .HasForeignKey(x => x.ModuleId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.CreatedBy)
                .WithMany()
                .HasForeignKey(x => x.CreatedById)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasIndex(x => new { x.UserId, x.ModuleId })
                .IsUnique();
        });
    }
}
