using SpendSmart.Category.API.Models;

namespace SpendSmart.Category.API.Repositories;

public interface ICategoryRepository
{
    Task<IList<Models.Category>> FindAllForUserAsync(int userId);
    Task<bool> ExistsByNameAsync(int? userId, string name);
    Task AddAsync(Models.Category category);
    Task DeactivateCategoryAsync(int categoryId);
}
