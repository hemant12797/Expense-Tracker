using Microsoft.EntityFrameworkCore;
using SpendSmart.Budget.API.Data;

namespace SpendSmart.Budget.API.Repositories;

public class BudgetRepository : IBudgetRepository
{
    private readonly BudgetDbContext _context;

    public BudgetRepository(BudgetDbContext context)
    {
        _context = context;
    }

    public async Task<Models.Budget?> GetBudgetAsync(int userId, int? categoryId)
    {
        return await _context.Budgets
            .FirstOrDefaultAsync(b => b.UserId == userId && b.CategoryId == categoryId && b.IsActive);
    }

    public async Task<IList<Models.Budget>> GetAllForUserAsync(int userId)
    {
        return await _context.Budgets
            .Where(b => b.UserId == userId && b.IsActive)
            .ToListAsync();
    }

    public async Task AddAsync(Models.Budget budget)
    {
        await _context.Budgets.AddAsync(budget);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateSpentAmountAsync(int budgetId, decimal amountToAdd)
    {
        await _context.Budgets
            .Where(b => b.BudgetId == budgetId)
            .ExecuteUpdateAsync(s => s.SetProperty(b => b.SpentAmount, b => b.SpentAmount + amountToAdd));
    }

    public async Task ResetMonthlyBudgetsAsync()
    {
        await _context.Budgets
            .Where(b => b.Period == "MONTHLY" && b.IsActive)
            .ExecuteUpdateAsync(s => s.SetProperty(b => b.SpentAmount, 0));
    }

    public async Task DeleteAsync(int id)
    {
        var budget = await _context.Budgets.FindAsync(id);
        if (budget != null)
        {
            _context.Budgets.Remove(budget);
            await _context.SaveChangesAsync();
        }
    }
}
