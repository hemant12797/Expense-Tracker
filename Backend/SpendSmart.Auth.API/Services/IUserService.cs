using SpendSmart.Auth.API.DTOs;
using SpendSmart.Auth.API.Models;

namespace SpendSmart.Auth.API.Services;

public interface IUserService
{
    Task<User> RegisterAsync(RegisterDto dto);
    Task<string> LoginAsync(string email, string password, bool isAdminLogin = false, string? passkey = null);
    Task<string> GoogleLoginAsync(string idToken);
    Task UpdateCurrencyAsync(int userId, string currency);
    Task ChangePasswordAsync(int userId, string currentPassword, string newPassword);
    Task DeleteUserAsync(int userId);
    Task ToggleUserStatusAsync(int userId, bool isActive);
    Task DeleteAllUsersAsync(int currentAdminId);
}
