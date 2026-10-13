using Factorio.Application.Abstractions;

namespace Factorio.Infrastructure.Authentication;

/// <summary>Adapter: IPasswordHasher implemented with BCrypt.</summary>
public class BCryptPasswordHasher : IPasswordHasher
{
    public string Hash(string password) => BCrypt.Net.BCrypt.HashPassword(password);

    public bool Verify(string password, string hash) => BCrypt.Net.BCrypt.Verify(password, hash);
}
