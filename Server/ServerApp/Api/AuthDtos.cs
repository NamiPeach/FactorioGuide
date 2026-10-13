using System.ComponentModel.DataAnnotations;

namespace ServerApp.Api;

/// <summary>Body of POST /api/auth/login.</summary>
public class LoginRequest
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

/// <summary>Response of POST /api/auth/login: send the token in the header "Authorization: Bearer TOKEN".</summary>
public record LoginResponse(string Token, DateTime ExpiresAtUtc);
