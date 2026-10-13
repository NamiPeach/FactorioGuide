using Factorio.Domain.Entities;

namespace Factorio.Application.Abstractions;

/// <summary>Port: access to items for the locale import. The EF Core adapter lives in Infrastructure.</summary>
public interface IItemLocaleRepository
{
    /// <summary>Tracked items that no approved user action has changed yet (UserId is null).</summary>
    Task<List<Item>> GetImportableItemsAsync();

    Task SaveChangesAsync();
}
