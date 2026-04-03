using MyAppTemplate.App.Configuration;
using MyAppTemplate.App.Hubs;
using MyAppTemplate.App.Services.BackgroundServices;
using MyAppTemplate.Contract.Interfaces;
using MyAppTemplate.Contract.Models;
using MyAppTemplate.Data.Context;
using MyAppTemplate.Data.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Get Current Process ID for log file naming
int processId = Environment.ProcessId;
int processId2 = System.Diagnostics.Process.GetCurrentProcess().Id; // Alternative way to get Process ID

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File(
        $"Logs/myapptemplate-{processId}-.txt",
        rollingInterval: RollingInterval.Day, // Creates a new file every day
        retainedFileCountLimit: 7, // Keeps only the last 7 days of logs
        rollOnFileSizeLimit: true, // Rolls when file gets too big (default 1GB)
        fileSizeLimitBytes: 50 * 1024 * 1024, // e.g., 50 MB per file (adjust as needed)
        shared: false) // Usually false for web apps to avoid file locking issues
    .CreateLogger();

builder.Host.UseSerilog();

try
{
    builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = null;
            options.JsonSerializerOptions.DictionaryKeyPolicy = null;
        });

    // Add SignalR for real-time updates
    builder.Services.AddSignalR()
        .AddJsonProtocol(options =>
        {
            options.PayloadSerializerOptions.PropertyNamingPolicy = null;
            options.PayloadSerializerOptions.DictionaryKeyPolicy = null;
        });

    // Add Infastructure
    builder.Services.AddInfrastructure(builder.Configuration);

    // Add services to the container.
    builder.Services.AddControllersWithViews();

    // Add Data Services
    builder.Services.AddDataServices();

    builder.Services.AddHostedService<SystemMonitoringService>();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    // Map the Hub URL
    app.MapHub<SystemHub>("/systemHub");

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapStaticAssets();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
        .WithStaticAssets();


    // Seed Data
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            var authService = services.GetRequiredService<IAuthenticationService>();
            var settings = services.GetRequiredService<IOptions<InitialSystemSettings>>().Value;

            // Check if there are any migrations that HAVEN'T been applied yet
            var pending = context.Database.GetPendingMigrations().Any();

            if (pending)
            {
                logger.LogWarning("Database is out of sync. Running migrations...");
                context.Database.Migrate();
            }

            logger.LogInformation("Proceeding with seeding...");
            SeedData.Initialize(context, authService, settings);
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "DATABASE INITIALIZATION FAILED.");
        }
    }

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "The application failed to start correctly.");
}
finally
{
    Log.CloseAndFlush();
}

