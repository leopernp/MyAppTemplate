using MyAppTemplate.Contract.Interfaces;
using MyAppTemplate.Contract.Models;
using MyAppTemplate.Data.Context;
using MyAppTemplate.Data.ContextModels;
using System.Runtime.CompilerServices;

namespace MyAppTemplate.Data.Helpers;

public static class SeedData
{
    public static void Initialize(
        ApplicationDbContext context, 
        IAuthenticationService authService,
        InitialSystemSettings settings)
    {
        // v1.0 — Initial seed
        const string v = "v1.0";
        ApplySeed(context, $"{v}-Roles", () => CreateRoles(context));
        ApplySeed(context, $"{v}-Admin", () => CreateAdmin(context, authService, settings));

        // v1.1 — Module system seed
        const string v2 = "v1.1";
        ApplySeed(context, $"{v2}-ModuleTypes", () => CreateModuleTypes(context));
        ApplySeed(context, $"{v2}-Modules", () => CreateModules(context));
        ApplySeed(context, $"{v2}-AdminPermissions", () => CreateAdminPermissions(context));
    }

    #region Private Helper Methods

    private static void ApplySeed(ApplicationDbContext context, string fullVersionKey, Action seedAction)
    {
        if (context.SeedHistories.Any(sh => sh.Version == fullVersionKey))
        {
            return;
        }

        using var transaction = context.Database.BeginTransaction();
        try
        {
            seedAction();

            context.SeedHistories.Add(new SeedHistory
            {
                Version = fullVersionKey,
                AppliedOn = DateTime.UtcNow
            });

            context.SaveChanges();
            transaction.Commit();
        }
        catch (Exception)
        {
            transaction.Rollback();
            throw;
        }
    }

    #endregion

    #region Create Methods

    private static void CreateRoles(ApplicationDbContext context)
    {
        // Seed Roles first
        if (!context.Roles.Any())
        {
            var roles = new Role[]
            {
                new Role
                {
                    Name = "Administrator",
                    IsActive = true,
                    CreatedById = null,
                    CreatedDate = DateTime.UtcNow
                },
                new Role
                {
                    Name = "User",
                    IsActive = true,
                    CreatedById = null,
                    CreatedDate = DateTime.UtcNow
                }
            };

            context.Roles.AddRange(roles);
            context.SaveChanges();
        }
    }

    private static void CreateAdmin(
        ApplicationDbContext context, 
        IAuthenticationService authenticationService,
        InitialSystemSettings settings)
    {
        // Seed System Admin User
        if (!context.Users.Any())
        {
            var adminRole = context.Roles.FirstOrDefault(r => r.Name == "Administrator");
            if (adminRole != null)
            {
                var adminUser = new User
                {
                    Username = "admin",
                    Password = authenticationService.HashPassword("Password@123"), // Hash the password
                    FirstName = "Super",
                    LastName = "Administrator",
                    Email = settings.AdminEmail,
                    RoleId = adminRole.Id,
                    IsActive = true,
                    IsSuperAdmin = true,
                    CreatedById = null, // First user has no creator
                    CreatedDate = DateTime.UtcNow
                };

                context.Users.Add(adminUser);
                context.SaveChanges();
            }
        }
    }

    #endregion

    #region v1.1 — Module System Seed

    private static void CreateModuleTypes(ApplicationDbContext context)
    {
        if (!context.ModuleTypes.Any())
        {
            var moduleTypes = new ModuleType[]
            {
                new ModuleType
                {
                    Name = "Dashboard",
                    Description = "System dashboard and home page",
                    Icon = "bi bi-speedometer2",
                    DisplayOrder = 1,
                    IsActive = true,
                    InMaintenance = false,
                    Controller = "Home",
                    Action = "Index",
                    CreatedById = null,
                    CreatedDate = DateTime.UtcNow
                },
                new ModuleType
                {
                    Name = "Transactions",
                    Description = "Transaction management",
                    Icon = "bi bi-arrow-left-right",
                    DisplayOrder = 2,
                    IsActive = true,
                    InMaintenance = false,
                    Controller = null,
                    Action = null,
                    CreatedById = null,
                    CreatedDate = DateTime.UtcNow
                },
                new ModuleType
                {
                    Name = "File Setups",
                    Description = "File and data setup management",
                    Icon = "bi bi-folder2-open",
                    DisplayOrder = 3,
                    IsActive = true,
                    InMaintenance = false,
                    Controller = null,
                    Action = null,
                    CreatedById = null,
                    CreatedDate = DateTime.UtcNow
                },
                new ModuleType
                {
                    Name = "Reports",
                    Description = "System reports and analytics",
                    Icon = "bi bi-bar-chart-line",
                    DisplayOrder = 4,
                    IsActive = true,
                    InMaintenance = false,
                    Controller = null,
                    Action = null,
                    CreatedById = null,
                    CreatedDate = DateTime.UtcNow
                },
                new ModuleType
                {
                    Name = "Settings",
                    Description = "System configuration and administration",
                    Icon = "bi bi-gear",
                    DisplayOrder = 5,
                    IsActive = true,
                    InMaintenance = false,
                    Controller = null,
                    Action = null,
                    CreatedById = null,
                    CreatedDate = DateTime.UtcNow
                }
            };
            context.ModuleTypes.AddRange(moduleTypes);
            context.SaveChanges();
        }
    }

    private static void CreateModules(ApplicationDbContext context)
    {
        if (!context.Modules.Any())
        {
            var settingsType = context.ModuleTypes.First(x => x.Name == "Settings");

            var modules = new Module[]
            {
                new Module
                {
                    Name = "Users",
                    Description = "User management",
                    Icon = "bi bi-people",
                    DisplayOrder = 1,
                    IsActive = true,
                    InMaintenance = false,
                    Controller = "User",
                    Action = "Index",
                    ModuleTypeId = settingsType.Id,
                    ParentModuleId = null,
                    CreatedById = null,
                    CreatedDate = DateTime.UtcNow
                },
                new Module
                {
                    Name = "Roles",
                    Description = "Role management",
                    Icon = "bi bi-shield-lock",
                    DisplayOrder = 2,
                    IsActive = true,
                    InMaintenance = false,
                    Controller = "Role",
                    Action = "Index",
                    ModuleTypeId = settingsType.Id,
                    ParentModuleId = null,
                    CreatedById = null,
                    CreatedDate = DateTime.UtcNow
                },
                new Module
                {
                    Name = "Module Types",
                    Description = "Module type management",
                    Icon = "bi bi-collection",
                    DisplayOrder = 3,
                    IsActive = true,
                    InMaintenance = false,
                    Controller = "ModuleType",
                    Action = "Index",
                    ModuleTypeId = settingsType.Id,
                    ParentModuleId = null,
                    CreatedById = null,
                    CreatedDate = DateTime.UtcNow
                },
                new Module
                {
                    Name = "Modules",
                    Description = "Module management",
                    Icon = "bi bi-grid",
                    DisplayOrder = 4,
                    IsActive = true,
                    InMaintenance = false,
                    Controller = "Module",
                    Action = "Index",
                    ModuleTypeId = settingsType.Id,
                    ParentModuleId = null,
                    CreatedById = null,
                    CreatedDate = DateTime.UtcNow
                }
            };
            context.Modules.AddRange(modules);
            context.SaveChanges();
        }
    }

    private static void CreateAdminPermissions(ApplicationDbContext context)
    {
        var adminRole = context.Roles.FirstOrDefault(r => r.Name == "Administrator");
        if (adminRole == null) return;

        var modules = context.Modules.ToList();
        if (!modules.Any()) return;

        // Only seed if no permissions exist for admin role yet
        if (context.RoleModulePermissions.Any(x => x.RoleId == adminRole.Id)) return;

        var permissions = modules.Select(module => new RoleModulePermission
        {
            RoleId = adminRole.Id,
            ModuleId = module.Id,
            CanView = true,
            CanCreate = true,
            CanUpdate = true,
            CanDelete = true,
            CreatedById = null,
            CreatedDate = DateTime.UtcNow
        }).ToArray();

        context.RoleModulePermissions.AddRange(permissions);
        context.SaveChanges();
    }

    #endregion
}
