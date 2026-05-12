namespace SpendSmart.Income.API.Repositories;

public interface IIncomeRepository
{
    Task<IList<Models.Income>> GetAllForUserAsync(int userId);
    Task<Models.Income?> GetByIdAsync(int id);
    Task<decimal> SumByUserIdAsync(int userId);
    Task AddAsync(Models.Income income);
    Task UpdateAsync(Models.Income income);
    Task DeleteAsync(int id);
}
