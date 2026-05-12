using SpendSmart.Category.API.Models;

namespace SpendSmart.Category.API.Services;

public interface ICategoryService
{
    Task SeedDefaultCategoriesAsync(int userId);
    Task<IList<Models.Category>> GetAllForUserAsync(int userId);
}
