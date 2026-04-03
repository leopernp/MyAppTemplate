using MyAppTemplate.App.Mapping;
using MyAppTemplate.App.Services;
using MyAppTemplate.Contract.Interfaces;
using MyAppTemplate.Contract.Models;
using MyAppTemplate.Data.Context;
using MyAppTemplate.Data.Mapping;
using MyAppTemplate.Data.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace MyAppTemplate.App.Configuration;

public static class ServiceRegistration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Bind settings sections
        services.Configure<InitialSystemSettings>(
            configuration.GetSection(InitialSystemSettings.SectionName));
        services.Configure<JwtSettings>(
            configuration.GetSection(JwtSettings.SectionName));

        // Database
        var connectionString = configuration.GetConnectionString("Default");
        services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

        // Memory cache
        services.AddMemoryCache();

        // Mapping profiles
        RegisterMappingProfiles(services);

        // Authentication
        RegisterAuthentication(services, configuration);

        services.AddAuthorization();

        return services;
    }

    public static IServiceCollection AddDataServices(this IServiceCollection services)
    {
        // Repositories — auto-discovered via reflection
        RegisterBaseRepositories(services);
        services.AddScoped<IDatabaseRepository, Data.Repositories.Tools.DatabaseRepository>();

        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<ISettingsService, SettingsService>();
        services.AddScoped<IUserActivityLogService, UserActivityLogService>();
        services.AddScoped<IModuleService, ModuleService>();
        services.AddScoped<IMenuCacheService, MenuCacheService>();
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<IDatabaseService, DatabaseService>();
        services.AddScoped<ISystemInfoService, SystemInfoService>();

        return services;
    }

    #region Private Helper Methods

    private static void RegisterAuthentication(IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration
            .GetSection(JwtSettings.SectionName)
            .Get<JwtSettings>() ?? new JwtSettings();

        services.AddAuthentication(options =>
        {
            // Cookie is the default for MVC UI
            options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        })
        .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
        {
            options.LoginPath = configuration["CookieSettings:LoginPath"] ?? "/Account/Login";
            options.LogoutPath = configuration["CookieSettings:LogoutPath"] ?? "/Account/Logout";
            options.AccessDeniedPath = configuration["CookieSettings:AccessDeniedPath"] ?? "/Account/AccessDenied";
            options.ExpireTimeSpan = TimeSpan.FromDays(
                configuration.GetValue<int>("CookieSettings:ExpireTimeSpanDays", 14));
            options.SlidingExpiration = true;
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.SameSite = SameSiteMode.Strict;
        })
        .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                ClockSkew = TimeSpan.Zero
            };
        });
    }

    private static void RegisterMappingProfiles(IServiceCollection services)
    {
        services.AddAutoMapper(
            typeof(IDataMappingMarker).Assembly,
            typeof(IWebMappingMarker).Assembly
        );
    }

    private static void RegisterBaseRepositories(IServiceCollection services)
    {
        var baseRepositoryType = typeof(BaseRepository<>);
        var repositoryTypes = baseRepositoryType.Assembly
            .GetTypes()
            .Where(type => type.IsClass
                && !type.IsAbstract
                && IsDerivedFromGeneric(type, baseRepositoryType));

        foreach (var repositoryType in repositoryTypes)
        {
            var contractInterfaces = repositoryType.GetInterfaces()
                .Where(@interface => @interface.Namespace == "MyAppTemplate.Contract.Interfaces"
                    && @interface.Name.EndsWith("Repository", StringComparison.Ordinal));

            foreach (var contractInterface in contractInterfaces)
            {
                services.AddScoped(contractInterface, repositoryType);
            }
        }
    }

    private static bool IsDerivedFromGeneric(Type type, Type genericBaseType)
    {
        var current = type;
        while (current != null && current != typeof(object))
        {
            var comparisonType = current.IsGenericType ? current.GetGenericTypeDefinition() : current;
            if (comparisonType == genericBaseType)
                return true;
            current = current.BaseType;
        }
        return false;
    }

    #endregion
}