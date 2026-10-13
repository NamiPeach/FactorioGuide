using Factorio.Domain.Entities;

namespace Factorio.Application.Abstractions;

/// <summary>Port: storage of items. The EF Core adapter lives in Infrastructure.</summary>
public interface IItemRepository
{
    Task<List<Item>> GetAllAsync(string? search = null);
    Task<Item?> GetByIdAsync(int id);
    Task<bool> InternalNameExistsAsync(string internalName, int? excludeId = null);

    /// <summary>True if the item has recipe rows or user actions attached to it.</summary>
    Task<bool> HasDependenciesAsync(int id);

    Task AddAsync(Item item);
    Task SaveChangesAsync();
    Task DeleteAsync(Item item);
}
