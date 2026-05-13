using Microsoft.EntityFrameworkCore;
using SpendSmart.Category.API.Data;
using SpendSmart.Category.API.Models;

namespace SpendSmart.Category.API.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly CategoryDbContext _context;

    public CategoryRepository(CategoryDbContext context)
    {
        _context = context;
    }

    public async Task<IList<Models.Category>> FindAllForUserAsync(int userId)
    {
        return await _context.Categories
            .Where(c => (c.UserId == null || c.UserId == userId) && c.IsActive)
            .OrderBy(c => c.IsDefault).ThenBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<bool> ExistsByNameAsync(int? userId, string name)
    {
        return await _context.Categories
            .AnyAsync(c => c.UserId == userId && c.Name == name);
    }

    public async Task AddAsync(Models.Category category)
    {
        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();
    }

    public async Task DeactivateCategoryAsync(int categoryId)
    {
        await _context.Categories
            .Where(c => c.CategoryId == categoryId)
            .ExecuteUpdateAsync(s => s.SetProperty(c => c.IsActive, false));
    }
}
