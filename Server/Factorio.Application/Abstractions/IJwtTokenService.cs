namespace Factorio.Application.Abstractions;

/// <summary>A signed JWT and the moment it stops being valid.</summary>
public record AccessToken(string Token, DateTime ExpiresAtUtc);

/// <summary>
/// Port: creates JWT access tokens.
/// Defined in the Application layer, implemented in the Infrastructure layer.
/// </summary>
public interface IJwtTokenService
{
    /// <summary>Creates a signed token for the given subject (login or user id) and role.</summary>
    AccessToken CreateToken(string subject, string role);
}