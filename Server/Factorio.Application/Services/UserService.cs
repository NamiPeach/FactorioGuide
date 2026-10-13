using Factorio.Application.Abstractions;
using Factorio.Application.Common;
using Factorio.Domain.Entities;

namespace Factorio.Application.Services;

/// <summary>Use cases for managing registered users (admin panel and, later, the API).</summary>
public class UserService(IUserRepository users, IPasswordHasher hasher)
{
    public const int MinPasswordLength = 6;

    public Task<List<User>> GetAllAsync() => users.GetAllAsync();

    public Task<User?> GetAsync(int id) => users.GetByIdAsync(id);

    public async Task<Result> CreateAsync(string name, string email, string? password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return Result.Fail("Password", "Password is required.");
        if (password.Length < MinPasswordLength)
            return Result.Fail("Password", $"Password must be at least {MinPasswordLength} characters.");

        var normalizedEmail = email.Trim().ToLowerInvariant();
        if (await users.EmailExistsAsync(normalizedEmail))
            return Result.Fail("Email", "A user with this email already exists.");

        await users.AddAsync(new User
        {
            Name = name.Trim(),
            Email = normalizedEmail,
            PasswordHash = hasher.Hash(password),
            CreatedAt = DateTime.UtcNow
        });
        return Result.Ok();
    }

    public async Task<Result> UpdateAsync(int id, string name, string email, string? newPassword)
    {
        var user = await users.GetByIdAsync(id);
        if (user is null) return Result.NotFound();

        var normalizedEmail = email.Trim().ToLowerInvariant();
        if (await users.EmailExistsAsync(normalizedEmail, id))
            return Result.Fail("Email", "A user with this email already exists.");

        var changePassword = !string.IsNullOrWhiteSpace(newPassword);
        if (changePassword && newPassword!.Length < MinPasswordLength)
            return Result.Fail("Password", $"Password must be at least {MinPasswordLength} characters.");

        user.Name = name.Trim();
        user.Email = normalizedEmail;
        if (changePassword)
            user.PasswordHash = hasher.Hash(newPassword!);

        await users.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result> DeleteAsync(int id)
    {
        var user = await users.GetByIdAsync(id);
        if (user is null) return Result.NotFound();

        await users.DeleteAsync(user);
        return Result.Ok();
    }
}
