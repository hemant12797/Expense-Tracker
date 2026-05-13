namespace SpendSmart.Budget.API.Services;

public interface IBudgetService
{
    Task CheckBudgetOnExpenseAsync(int userId, int categoryId, decimal amount);

    /// <summary>
    /// Returns null if valid, or an error message if the expense would exceed the budget.
    /// </summary>
    Task<string?> ValidateBudgetAsync(int userId, int? categoryId, decimal amount);
}
