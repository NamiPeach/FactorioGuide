using Factorio.Application.Abstractions;
using Factorio.Domain.Entities;
using Factorio.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Factorio.Infrastructure.Repositories;

/// <summary>Adapter: IUserRepository implemented with EF Core.</summary>
public class UserRepository(AppDbContext db) : IUserRepository
{
    public Task<List<User>> GetAllAsync() =>
        db.Users.AsNoTracking().OrderBy(u => u.Name).ToListAsync();

    public Task<User?> GetByIdAsync(int id) =>
        db.Users.FirstOrDefaultAsync(u => u.Id == id);

    public Task<bool> EmailExistsAsync(string normalizedEmail, int? excludeId = null) =>
        db.Users.AnyAsync(u => u.Email.ToLower() == normalizedEmail
                               && (excludeId == null || u.Id != excludeId.Value));

    public Task<User?> GetByEmailAsync(string normalizedEmail) =>
        db.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail);

    public async Task AddAsync(User user)
    {
        db.Users.Add(user);
        await db.SaveChangesAsync();
    }

    public Task SaveChangesAsync() => db.SaveChangesAsync();

    public async Task DeleteAsync(User user)
    {
        db.Users.Remove(user);
        await db.SaveChangesAsync();
    }
}

