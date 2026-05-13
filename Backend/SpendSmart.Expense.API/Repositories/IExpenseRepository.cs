namespace SpendSmart.Expense.API.Repositories;

public interface IExpenseRepository
{
    Task<Models.Expense?> GetByIdAsync(int id);
    Task<IList<Models.Expense>> GetAllForUserAsync(int userId);
    Task<decimal> SumByUserIdAsync(int userId);
    Task<IList<Models.Expense>> SearchExpensesAsync(int userId, string keyword);
    Task AddAsync(Models.Expense expense);
    Task UpdateAsync(Models.Expense expense);
    Task DeleteAsync(int id);
}
