namespace Factorio.Application.Abstractions;

/// <summary>Port: password hashing. The adapter lives in Infrastructure.</summary>
public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}
