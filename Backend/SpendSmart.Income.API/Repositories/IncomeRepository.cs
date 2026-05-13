using Microsoft.EntityFrameworkCore;
using SpendSmart.Income.API.Data;

namespace SpendSmart.Income.API.Repositories;

public class IncomeRepository : IIncomeRepository
{
    private readonly IncomeDbContext _context;

    public IncomeRepository(IncomeDbContext context)
    {
        _context = context;
    }

    public async Task<IList<Models.Income>> GetAllForUserAsync(int userId)
    {
        return await _context.Incomes
            .Where(i => i.UserId == userId)
            .OrderByDescending(i => i.Date)
            .ToListAsync();
    }

    public async Task<Models.Income?> GetByIdAsync(int id)
    {
        return await _context.Incomes.FindAsync(id);
    }

    public async Task<decimal> SumByUserIdAsync(int userId)
    {
        return await _context.Incomes
            .Where(i => i.UserId == userId)
            .SumAsync(i => i.Amount);
    }

    public async Task AddAsync(Models.Income income)
    {
        await _context.Incomes.AddAsync(income);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Models.Income income)
    {
        _context.Incomes.Update(income);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var income = await _context.Incomes.FindAsync(id);
        if (income != null)
        {
            _context.Incomes.Remove(income);
            await _context.SaveChangesAsync();
        }
    }
}
