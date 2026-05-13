using System.Net.Http;
using System.Text.Json;
using System.Text;
using SpendSmart.Budget.API.Repositories;
using SpendSmart.Shared.Messages;

namespace SpendSmart.Budget.API.Services;

public class BudgetService : IBudgetService
{
    private readonly IBudgetRepository _repo;
    private readonly ILogger<BudgetService> _logger;
    private readonly HttpClient _httpClient;

    public BudgetService(IBudgetRepository repo, ILogger<BudgetService> logger, HttpClient httpClient)
    {
        _repo = repo;
        _logger = logger;
        _httpClient = httpClient;
    }

    public async Task CheckBudgetOnExpenseAsync(int userId, int categoryId, decimal amount)
    {
        // Check specific category budget
        var categoryBudget = await _repo.GetBudgetAsync(userId, categoryId);
        if (categoryBudget != null)
        {
            await ProcessBudget(categoryBudget, amount, userId);
        }

        // Check overall budget (CategoryId = null)
        var overallBudget = await _repo.GetBudgetAsync(userId, null);
        if (overallBudget != null)
        {
            await ProcessBudget(overallBudget, amount, userId);
        }
    }

    public async Task<string?> ValidateBudgetAsync(int userId, int? categoryId, decimal amount)
    {
        // Check category-specific budget
        if (categoryId.HasValue && categoryId.Value > 0)
        {
            var catBudget = await _repo.GetBudgetAsync(userId, categoryId.Value);
            if (catBudget != null)
            {
                var remaining = catBudget.LimitAmount - catBudget.SpentAmount;
                if (amount > remaining)
                    return $"Budget limit exceeded for this category! Remaining: ₹{remaining:N2}, You tried to add: ₹{amount:N2}.";
            }
        }

        // Check overall budget (no category)
        var overallBudget = await _repo.GetBudgetAsync(userId, null);
        if (overallBudget != null)
        {
            var remaining = overallBudget.LimitAmount - overallBudget.SpentAmount;
            if (amount > remaining)
                return $"Overall budget limit exceeded! Remaining: ₹{remaining:N2}, You tried to add: ₹{amount:N2}.";
        }

        return null; // valid
    }

    private async Task ProcessBudget(Models.Budget budget, decimal amount, int userId)
    {
        await _repo.UpdateSpentAmountAsync(budget.BudgetId, amount);
        
        var newSpentAmount = budget.SpentAmount + amount;
        
        if (newSpentAmount >= budget.LimitAmount)
        {
            // Exceeded threshold
            _logger.LogWarning($"Budget EXCEEDED for BudgetId {budget.BudgetId}. Limit: {budget.LimitAmount}, Spent: {newSpentAmount}");
            await SendNotificationAsync(userId, "Budget Exceeded!", $"You have exceeded your budget '{budget.Name}'. Limit: {budget.LimitAmount:C}, Spent: {newSpentAmount:C}.");
        }
        else if (newSpentAmount >= (budget.LimitAmount * 0.8m))
        {
            // Warning threshold
            _logger.LogInformation($"Budget WARNING for BudgetId {budget.BudgetId}. Limit: {budget.LimitAmount}, Spent: {newSpentAmount}");
            await SendNotificationAsync(userId, "Budget Warning", $"You have spent 80% or more of your budget '{budget.Name}'. Limit: {budget.LimitAmount:C}, Spent: {newSpentAmount:C}.");
        }
    }

    private async Task SendNotificationAsync(int userId, string title, string message)
    {
        try
        {
            var payload = new { userId, title, message };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            await _httpClient.PostAsync("http://localhost:5207/api/notifications", content);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send notification via HTTP");
        }
    }
}
