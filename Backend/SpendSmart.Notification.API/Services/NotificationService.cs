using Microsoft.AspNetCore.SignalR;
using SpendSmart.Notification.API.DTOs;
using SpendSmart.Notification.API.Hubs;
using SpendSmart.Notification.API.Repositories;

namespace SpendSmart.Notification.API.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _repo;
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        INotificationRepository repo,
        IHubContext<NotificationHub> hubContext,
        ILogger<NotificationService> logger)
    {
        _repo = repo;
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task<Models.Notification> CreateAsync(CreateNotificationDto dto)
    {
        var notification = new Models.Notification
        {
            UserId = dto.UserId,
            Title = dto.Title,
            Message = dto.Message,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        // Save to DB
        await _repo.AddAsync(notification);

        // Push via SignalR to the user if connected
        try
        {
            await _hubContext.Clients
                .Group($"User_{dto.UserId}")
                .SendAsync("ReceiveNotification", notification);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "SignalR push failed for UserId {UserId}", dto.UserId);
        }

        return notification;
    }

    public async Task<IList<Models.Notification>> GetAllForUserAsync(int userId)
    {
        return await _repo.GetUnreadForUserAsync(userId);
    }
}
