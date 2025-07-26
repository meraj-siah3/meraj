using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MessageDistributor.Services;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly HealthCheckService _healthCheckService;

    private readonly string _engineId = Guid.NewGuid().ToString();

    public Worker(ILogger<Worker> logger, HealthCheckService healthCheckService)
    {
        _logger = logger;
        _healthCheckService = healthCheckService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var result = await _healthCheckService.CheckHealthAsync(_engineId, connectedClients: 5);

            if (result is not null)
            {
                _logger.LogInformation(" HealthCheck Result → IsEnabled: {status}", result.IsEnabled);
            }
            else
            {
                _logger.LogWarning(" HealthCheck failed or returned null.");
            }

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}
 