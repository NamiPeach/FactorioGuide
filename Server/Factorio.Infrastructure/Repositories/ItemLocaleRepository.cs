using Factorio.Application.Abstractions;
using Factorio.Domain.Entities;
using Factorio.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Factorio.Infrastructure.Repositories;

/// <summary>Adapter: IItemLocaleRepository implemented with EF Core.</summary>
public class ItemLocaleRepository(AppDbContext db) : IItemLocaleRepository
{
    public Task<List<Item>> GetImportableItemsAsync() =>
        db.Set<Item>().Where(i => i.UserId == null).ToListAsync();

    public Task SaveChangesAsync() => db.SaveChangesAsync();
}
