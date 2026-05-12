namespace SpendSmart.Auth.API.DTOs;

public class LoginDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool IsAdminLogin { get; set; }
    public string? Passkey { get; set; }
}
