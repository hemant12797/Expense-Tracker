using SpendSmart.Budget.API.Repositories;

namespace SpendSmart.Budget.API.Services;

public class BudgetResetService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<BudgetResetService> _logger;

    public BudgetResetService(IServiceProvider serviceProvider, ILogger<BudgetResetService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.UtcNow;
            
            // Calculate delay until the 1st of the next month at midnight
            var firstOfNextMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc).AddMonths(1);
            var delay = firstOfNextMonth - now;

            _logger.LogInformation($"BudgetResetService waiting {delay.TotalHours} hours until next reset.");
            
            // Wait until the next month
            await Task.Delay(delay, stoppingToken);

            if (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Resetting monthly budgets...");
                
                using var scope = _serviceProvider.CreateScope();
                var repo = scope.ServiceProvider.GetRequiredService<IBudgetRepository>();
                
                await repo.ResetMonthlyBudgetsAsync();
                
                _logger.LogInformation("Monthly budgets have been successfully reset.");
            }
        }
    }
}
