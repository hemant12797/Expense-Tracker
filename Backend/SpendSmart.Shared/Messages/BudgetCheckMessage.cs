namespace SpendSmart.Shared.Messages;

public record BudgetCheckMessage(int UserId, int CategoryId, decimal Amount, int ExpenseId);
