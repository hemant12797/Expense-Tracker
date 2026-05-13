using SpendSmart.Income.API.Repositories;

namespace SpendSmart.Income.API.Services;

public class IncomeService : IIncomeService
{
    private readonly IIncomeRepository _repo;

    public IncomeService(IIncomeRepository repo)
    {
        _repo = repo;
    }

    public async Task<Models.Income> AddIncomeAsync(Models.Income income)
    {
        income.CreatedAt = DateTime.UtcNow;
        await _repo.AddAsync(income);
        return income;
    }
}
