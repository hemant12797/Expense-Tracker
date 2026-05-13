using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SpendSmart.Auth.API.DTOs;
using SpendSmart.Auth.API.Repositories;
using SpendSmart.Auth.API.Services;

namespace SpendSmart.Auth.API.Controllers;

[Route("api/users")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IUserRepository _userRepository;

    public UserController(IUserService userService, IUserRepository userRepository)
    {
        _userService = userService;
        _userRepository = userRepository;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        try
        {
            var user = await _userService.RegisterAsync(dto);
            return CreatedAtAction(nameof(GetProfile), new { id = user.UserId }, user);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        try
        {
            var token = await _userService.LoginAsync(dto.Email, dto.Password, dto.IsAdminLogin, dto.Passkey);
            return Ok(new { token });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    [HttpPost("google-login")]
    public async Task<IActionResult> GoogleLogin(GoogleAuthDto dto)
    {
        try
        {
            var token = await _userService.GoogleLoginAsync(dto.IdToken);
            return Ok(new { token });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetProfile(int id)
    {
        var user = await _userRepository.FindByIdAsync(id);
        if (user == null) return NotFound();
        return Ok(user);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userRepository.GetAllAsync();
        return Ok(users);
    }

    [HttpPut("{id}/currency")]
    [Authorize]
    public async Task<IActionResult> UpdateCurrency(int id, UpdateCurrencyDto dto)
    {
        try
        {
            await _userService.UpdateCurrencyAsync(id, dto.Currency);
            return Ok(new { message = "Currency updated successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}/password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword(int id, ChangePasswordDto dto)
    {
        try
        {
            await _userService.ChangePasswordAsync(id, dto.CurrentPassword, dto.NewPassword);
            return Ok(new { message = "Password updated successfully" });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        await _userService.DeleteUserAsync(id);
        return Ok(new { message = "User deleted successfully" });
    }

    [HttpPut("{id}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ToggleUserStatus(int id, UpdateStatusDto dto)
    {
        try
        {
            await _userService.ToggleUserStatusAsync(id, dto.IsActive);
            return Ok(new { message = "User status updated successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteAllUsers()
    {
        var adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        await _userService.DeleteAllUsersAsync(adminId);
        return Ok(new { message = "All non-admin users deleted successfully" });
    }
}
