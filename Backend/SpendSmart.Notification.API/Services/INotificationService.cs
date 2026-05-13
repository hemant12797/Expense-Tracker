using SpendSmart.Notification.API.DTOs;

namespace SpendSmart.Notification.API.Services;

public interface INotificationService
{
    Task<Models.Notification> CreateAsync(CreateNotificationDto dto);
    Task<IList<Models.Notification>> GetAllForUserAsync(int userId);
}
