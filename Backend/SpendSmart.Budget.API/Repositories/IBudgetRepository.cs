using SpendSmart.Budget.API.Models;

namespace SpendSmart.Budget.API.Repositories;

public interface IBudgetRepository
{
    Task<Models.Budget?> GetBudgetAsync(int userId, int? categoryId);
    Task<IList<Models.Budget>> GetAllForUserAsync(int userId);
    Task AddAsync(Models.Budget budget);
    Task UpdateSpentAmountAsync(int budgetId, decimal amountToAdd);
    Task ResetMonthlyBudgetsAsync();
    Task DeleteAsync(int id);
}
