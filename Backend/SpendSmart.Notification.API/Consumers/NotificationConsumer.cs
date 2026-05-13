using MassTransit;
using Microsoft.AspNetCore.SignalR;
using SpendSmart.Notification.API.Hubs;
using SpendSmart.Notification.API.Repositories;
using SpendSmart.Notification.API.Services;
using SpendSmart.Shared.Messages;

namespace SpendSmart.Notification.API.Consumers;

public class NotificationConsumer : IConsumer<NotificationMessage>
{
    private readonly INotificationRepository _repo;
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly IEmailService _emailService;

    public NotificationConsumer(
        INotificationRepository repo,
        IHubContext<NotificationHub> hubContext,
        IEmailService emailService)
    {
        _repo = repo;
        _hubContext = hubContext;
        _emailService = emailService;
    }

    public async Task Consume(ConsumeContext<NotificationMessage> context)
    {
        var msg = context.Message;

        // 1. Save to DB
        var notification = new Models.Notification
        {
            UserId = msg.UserId,
            Title = msg.Title,
            Message = msg.Message
        };
        await _repo.AddAsync(notification);

        // 2. Push via SignalR to active connections
        await _hubContext.Clients.Group($"User_{msg.UserId}")
            .SendAsync("ReceiveNotification", notification);

        // 3. Optional Email
        if (!string.IsNullOrEmpty(msg.Email))
        {
            await _emailService.SendEmailAsync(msg.Email, msg.Title, msg.Message);
        }
    }
}
