using SpendSmart.Category.API.Models;
using SpendSmart.Category.API.Repositories;

namespace SpendSmart.Category.API.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repo;

    private static readonly string[] DefaultExpenseCategories =
        { "Food", "Transport", "Entertainment", "Health", "Shopping", "Bills", "Education", "Travel" };
    
    private static readonly string[] DefaultIncomeCategories =
        { "Salary", "Freelance", "Investment", "Rental", "Other" };

    public CategoryService(ICategoryRepository repo)
    {
        _repo = repo;
    }

    public async Task SeedDefaultCategoriesAsync(int userId)
    {
        foreach (var name in DefaultExpenseCategories)
        {
            if (!await _repo.ExistsByNameAsync(null, name))
                await _repo.AddAsync(new Models.Category { UserId = null, Name = name, Type = "EXPENSE", IsDefault = true });
        }
        foreach (var name in DefaultIncomeCategories)
        {
            if (!await _repo.ExistsByNameAsync(null, name))
                await _repo.AddAsync(new Models.Category { UserId = null, Name = name, Type = "INCOME", IsDefault = true });
        }
    }

    public async Task<IList<Models.Category>> GetAllForUserAsync(int userId)
    {
        var categories = await _repo.FindAllForUserAsync(userId);
        if (!categories.Any(c => c.IsDefault))
        {
            await SeedDefaultCategoriesAsync(userId);
            categories = await _repo.FindAllForUserAsync(userId);
        }
        return categories;
    }
}
