namespace SpendSmart.Notification.API.Repositories;

public interface INotificationRepository
{
    Task<IList<Models.Notification>> GetUnreadForUserAsync(int userId);
    Task<IList<Models.Notification>> GetAllForUserAsync(int userId);
    Task AddAsync(Models.Notification notification);
    Task MarkAsReadAsync(int notificationId);
    Task MarkAllAsReadAsync(int userId);
}
