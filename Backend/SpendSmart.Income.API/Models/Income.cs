namespace SpendSmart.Income.API.Models;

public class Income
{
    public int IncomeId { get; set; }
    public int UserId { get; set; }
    public string Source { get; set; } = "SALARY"; // SALARY/FREELANCE/INVESTMENT/RENTAL/OTHER
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "INR";
    public string? Description { get; set; }
    public DateTime Date { get; set; }
    public bool IsRecurring { get; set; } = false;
    public string? RecurrenceType { get; set; } // MONTHLY/WEEKLY/YEARLY
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
