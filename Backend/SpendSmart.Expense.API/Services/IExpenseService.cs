namespace SpendSmart.Expense.API.Services;

public interface IExpenseService
{
    Task<Models.Expense> AddExpenseAsync(Models.Expense expense);
    Task<string> UploadReceiptAsync(IFormFile file, int userId);
}
