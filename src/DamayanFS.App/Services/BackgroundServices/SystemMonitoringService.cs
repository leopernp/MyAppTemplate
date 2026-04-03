using MyAppTemplate.App.Hubs;
using MyAppTemplate.Contract.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace MyAppTemplate.App.Services.BackgroundServices;

public class SystemMonitoringService : BackgroundService
{
    private readonly IHubContext<SystemHub> _hubContext;
    private readonly IServiceProvider _serviceProvider;

    public SystemMonitoringService(IHubContext<SystemHub> hubContext, IServiceProvider serviceProvider)
    {
        _hubContext = hubContext;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var systemService = scope.ServiceProvider.GetRequiredService<ISystemInfoService>();
                var stats = await systemService.GetSystemMetricsAsync();

                // Push to clients
                await _hubContext.Clients.All.SendAsync("ReceiveSystemUpdate", stats, stoppingToken);
            }

            await Task.Delay(1000, stoppingToken);
        }
    }
}
