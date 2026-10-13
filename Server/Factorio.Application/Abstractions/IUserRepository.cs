using Factorio.Domain.Entities;

namespace Factorio.Application.Abstractions;

/// <summary>Port: storage of users. The EF Core adapter lives in Infrastructure.</summary>
public interface IUserRepository
{
    Task<List<User>> GetAllAsync();
    Task<User?> GetByIdAsync(int id);
    Task<bool> EmailExistsAsync(string normalizedEmail, int? excludeId = null);
    Task AddAsync(User user);
    Task SaveChangesAsync();
    Task DeleteAsync(User user);
}
