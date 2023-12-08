using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DISProject.Database.Services.PurchaseServices;

public class PurchaseQueueProcessor : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly SemaphoreSlim _processQueueSemaphore = new(1, 1);
    public PurchaseQueueProcessor(IServiceProvider services) => _services = services;
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await _processQueueSemaphore.WaitAsync();
            try
            {
                // Call a method to process a queue
                using (var scope = _services.CreateScope())
                {
                    var purchaseService = scope.ServiceProvider.GetRequiredService<IPurchaseService>();
                    await purchaseService.ProcessQueueAsync();
                }
            }
            finally
            {
                _processQueueSemaphore.Release();
            }

            // Delay execution for a certain time interval
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}