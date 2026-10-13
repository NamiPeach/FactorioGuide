using Factorio.Application.Abstractions;
using Factorio.Domain.Entities;
using Factorio.Domain.Enums;
using Factorio.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Factorio.Infrastructure.Repositories;

/// <summary>Adapter: IActionRepository implemented with EF Core.</summary>
public class ActionRepository(AppDbContext db) : IActionRepository
{
    public Task<List<UserAction>> GetAllAsync(ActionStatus? status = null)
    {
        var query = db.Actions.AsNoTracking()
            .Include(a => a.User)
            .Include(a => a.Item)
            .AsQueryable();

        if (status is not null)
            query = query.Where(a => a.Status == status);

        return query.OrderByDescending(a => a.CreatedAt).ToListAsync();
    }

    public Task<UserAction?> GetByIdAsync(int id) =>
        db.Actions.Include(a => a.User).Include(a => a.Item).FirstOrDefaultAsync(a => a.Id == id);

    public async Task AddAsync(UserAction action)
    {
        db.Actions.Add(action);
        await db.SaveChangesAsync();
    }

    public Task SaveChangesAsync() => db.SaveChangesAsync();

    public async Task DeleteAsync(UserAction action)
    {
        db.Actions.Remove(action);
        await db.SaveChangesAsync();
    }
}
