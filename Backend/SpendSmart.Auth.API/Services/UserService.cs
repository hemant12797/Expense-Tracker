using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SpendSmart.Auth.API.DTOs;
using SpendSmart.Auth.API.Models;
using SpendSmart.Auth.API.Repositories;
using Google.Apis.Auth;

namespace SpendSmart.Auth.API.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repo;
    private readonly IConfiguration _config;

    public UserService(IUserRepository repo, IConfiguration config)
    {
        _repo = repo;
        _config = config;
    }

    public async Task<User> RegisterAsync(RegisterDto dto)
    {
        var existing = await _repo.FindByEmailAsync(dto.Email);
        if (existing != null)
            throw new Exception("Email already exists");

        var role = dto.Email.ToLower().Contains("admin") ? "Admin" : "User";
        var user = new User { FullName = dto.FullName, Email = dto.Email, Currency = dto.Currency, Role = role };
        var hasher = new PasswordHasher<User>();
        user.PasswordHash = hasher.HashPassword(user, dto.Password);
        
        await _repo.AddAsync(user);
        return user;
    }

    public async Task<string> LoginAsync(string email, string password, bool isAdminLogin = false, string? passkey = null)
    {
        var user = await _repo.FindByEmailAsync(email);
        if (user == null)
            throw new UnauthorizedAccessException("Invalid credentials");

        if (!user.IsActive)
            throw new UnauthorizedAccessException("Your account has been deactivated by an admin.");

        var hasher = new PasswordHasher<User>();
        var result = hasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (result == PasswordVerificationResult.Failed)
            throw new UnauthorizedAccessException("Invalid credentials");

        if (isAdminLogin)
        {
            if (passkey != "spend@admin")
                throw new UnauthorizedAccessException("Invalid Admin Passkey");
            
            user.Role = "Admin";
        }
        else
        {
            user.Role = "User"; // Force user role if not admin login
        }

        user.LastLoginAt = DateTime.UtcNow;
        await _repo.UpdateAsync(user);

        return GenerateJwtToken(user);
    }

    public async Task<string> GoogleLoginAsync(string idToken)
    {
        var settings = new GoogleJsonWebSignature.ValidationSettings
        {
            Audience = new[] { _config["Authentication:GoogleClientId"] }
        };

        GoogleJsonWebSignature.Payload payload;
        try
        {
            payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
        }
        catch (InvalidJwtException)
        {
            throw new UnauthorizedAccessException("Invalid Google token");
        }

        var user = await _repo.FindByEmailAsync(payload.Email);
        if (user == null)
        {
            user = new User
            {
                FullName = payload.Name ?? payload.Email,
                Email = payload.Email,
                Currency = "INR",
                AvatarUrl = payload.Picture,
                PasswordHash = string.Empty // No password for Google auth users
            };
            await _repo.AddAsync(user);
        }
        else if (!user.IsActive)
        {
            throw new UnauthorizedAccessException("Your account has been deactivated by an admin.");
        }

        user.LastLoginAt = DateTime.UtcNow;
        await _repo.UpdateAsync(user);

        return GenerateJwtToken(user);
    }

    public async Task UpdateCurrencyAsync(int userId, string currency)
    {
        var user = await _repo.FindByIdAsync(userId);
        if (user == null) throw new Exception("User not found");

        user.Currency = currency;
        await _repo.UpdateAsync(user);
    }

    public async Task ChangePasswordAsync(int userId, string currentPassword, string newPassword)
    {
        var user = await _repo.FindByIdAsync(userId);
        if (user == null) throw new Exception("User not found");

        var hasher = new PasswordHasher<User>();
        if (!string.IsNullOrEmpty(user.PasswordHash))
        {
            var result = hasher.VerifyHashedPassword(user, user.PasswordHash, currentPassword);
            if (result == PasswordVerificationResult.Failed)
                throw new UnauthorizedAccessException("Incorrect current password");
        }

        user.PasswordHash = hasher.HashPassword(user, newPassword);
        await _repo.UpdateAsync(user);
    }

    public async Task DeleteUserAsync(int userId)
    {
        var user = await _repo.FindByIdAsync(userId);
        if (user != null)
        {
            await _repo.DeleteAsync(user);
        }
    }

    public async Task ToggleUserStatusAsync(int userId, bool isActive)
    {
        var user = await _repo.FindByIdAsync(userId);
        if (user == null) throw new Exception("User not found");

        user.IsActive = isActive;
        await _repo.UpdateAsync(user);
    }

    public async Task DeleteAllUsersAsync(int currentAdminId)
    {
        await _repo.DeleteAllExceptAsync(currentAdminId);
    }

    private string GenerateJwtToken(User user)
    {
        var secretKey = _config["JwtSettings:SecretKey"];
        var issuer = _config["JwtSettings:Issuer"];
        var audience = _config["JwtSettings:Audience"];

        var claims = new[] 
        { 
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FullName ?? user.Email)
        };
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(double.Parse(_config["JwtSettings:ExpiryHours"] ?? "24")),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
