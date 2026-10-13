using Factorio.Application.Abstractions;

namespace Factorio.Application.Services;

/// <summary>Use case: a registered user logs in through the REST API.</summary>
public class AuthService(IUserRepository users, IPasswordHasher hasher, IJwtTokenService tokens)
{
    public const string UserRole = "User";

    /// <summary>Returns a token for valid credentials, or null if the email or password is wrong.</summary>
    public async Task<AccessToken?> LoginAsync(string email, string password)
    {
        var user = await users.GetByEmailAsync(email.Trim().ToLowerInvariant());
        if (user is null || !hasher.Verify(password, user.PasswordHash))
            return null;

        return tokens.CreateToken(user.Id.ToString(), UserRole);
    }
}
