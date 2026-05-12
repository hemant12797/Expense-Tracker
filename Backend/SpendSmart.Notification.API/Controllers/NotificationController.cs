using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpendSmart.Notification.API.DTOs;
using SpendSmart.Notification.API.Repositories;
using SpendSmart.Notification.API.Services;

namespace SpendSmart.Notification.API.Controllers;

[Route("api/notifications")]
[ApiController]
[Authorize]
public class NotificationController : ControllerBase
{
    private readonly INotificationRepository _repo;
    private readonly INotificationService _notificationService;

    public NotificationController(INotificationRepository repo, INotificationService notificationService)
    {
        _repo = repo;
        _notificationService = notificationService;
    }

    /// <summary>
    /// Ek nayi notification create karo (DB save + SignalR push)
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateNotificationDto dto)
    {
        if (dto.UserId <= 0 || string.IsNullOrWhiteSpace(dto.Title) || string.IsNullOrWhiteSpace(dto.Message))
            return BadRequest(new { message = "UserId, Title aur Message required hain." });

        var notification = await _notificationService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetAllForUser), new { userId = notification.UserId }, notification);
    }

    /// <summary>
    /// Kisi user ki saari notifications lo (read + unread)
    /// </summary>
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetAllForUser(int userId)
    {
        var list = await _repo.GetAllForUserAsync(userId);
        return Ok(list);
    }

    /// <summary>
    /// Kisi user ki sirf unread notifications lo
    /// </summary>
    [HttpGet("user/{userId}/unread")]
    public async Task<IActionResult> GetUnread(int userId)
    {
        var list = await _repo.GetUnreadForUserAsync(userId);
        return Ok(list);
    }

    /// <summary>
    /// Ek notification ko read mark karo
    /// </summary>
    [HttpPut("{id}/read")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        await _repo.MarkAsReadAsync(id);
        return NoContent();
    }

    /// <summary>
    /// User ki saari notifications ko read mark karo
    /// </summary>
    [HttpPut("user/{userId}/readAll")]
    public async Task<IActionResult> MarkAllAsRead(int userId)
    {
        await _repo.MarkAllAsReadAsync(userId);
        return NoContent();
    }
}
