namespace SpendSmart.Shared.Messages;

public record NotificationMessage(int UserId, string Title, string Message, string? Email = null);
