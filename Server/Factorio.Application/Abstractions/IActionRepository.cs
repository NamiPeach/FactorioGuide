using Factorio.Domain.Entities;
using Factorio.Domain.Enums;

namespace Factorio.Application.Abstractions;

/// <summary>Port: storage of user actions (requests). The EF Core adapter lives in Infrastructure.</summary>
public interface IActionRepository
{
    Task<List<UserAction>> GetAllAsync(ActionStatus? status = null);

    /// <summary>Tracked entity with User and Item loaded, so a review can change the item too.</summary>
    Task<UserAction?> GetByIdAsync(int id);

    Task AddAsync(UserAction action);
    Task SaveChangesAsync();
    Task DeleteAsync(UserAction action);
}
