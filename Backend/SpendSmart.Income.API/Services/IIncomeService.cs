namespace SpendSmart.Income.API.Services;

public interface IIncomeService
{
    Task<Models.Income> AddIncomeAsync(Models.Income income);
}
