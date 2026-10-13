using Factorio.Application.Abstractions;
using Factorio.Domain.Entities;
using Factorio.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Factorio.Infrastructure.Repositories;

/// <summary>Adapter: IItemRepository implemented with EF Core.</summary>
public class ItemRepository(AppDbContext db) : IItemRepository
{
    public Task<List<Item>> GetAllAsync(string? search = null)
    {
        var query = db.Set<Item>().AsNoTracking().Include(i => i.User).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLower();
            query = query.Where(i => i.Name.ToLower().Contains(s)
                                     || i.InternalName.ToLower().Contains(s)
                                     || (i.Category != null && i.Category.ToLower().Contains(s)));
        }

        return query.OrderBy(i => i.Name).ToListAsync();
    }

    public Task<Item?> GetByIdAsync(int id) =>
        db.Set<Item>().FirstOrDefaultAsync(i => i.Id == id);

    public Task<bool> InternalNameExistsAsync(string internalName, int? excludeId = null) =>
        db.Set<Item>().AnyAsync(i => i.InternalName.ToLower() == internalName
                                     && (excludeId == null || i.Id != excludeId.Value));

    public Task<bool> HasDependenciesAsync(int id) =>
        db.Set<Item>().Where(i => i.Id == id)
            .AnyAsync(i => i.Ingredients.Any() || i.UsedIn.Any() || i.Actions.Any());

    public async Task AddAsync(Item item)
    {
        db.Set<Item>().Add(item);
        await db.SaveChangesAsync();
    }

    public Task SaveChangesAsync() => db.SaveChangesAsync();

    public async Task DeleteAsync(Item item)
    {
        db.Set<Item>().Remove(item);
        await db.SaveChangesAsync();
    }
}
