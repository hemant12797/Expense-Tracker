namespace SpendSmart.Budget.API.Models;

public class Budget
{
    public int BudgetId { get; set; }
    public int UserId { get; set; }
    public int? CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal LimitAmount { get; set; }
    public decimal SpentAmount { get; set; } = 0;
    public string Currency { get; set; } = "INR";
    public string Period { get; set; } = "MONTHLY";
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; } = true;
}
