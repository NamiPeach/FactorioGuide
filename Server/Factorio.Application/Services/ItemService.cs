using System.Text.Json;
using Factorio.Application.Abstractions;
using Factorio.Application.Common;
using Factorio.Domain.Entities;

namespace Factorio.Application.Services;

/// <summary>Use cases for managing items (admin panel and, later, the API).</summary>
public class ItemService(IItemRepository items, IUserRepository users)
{
    public Task<List<Item>> GetAllAsync(string? search = null) => items.GetAllAsync(search);

    public Task<Item?> GetAsync(int id) => items.GetByIdAsync(id);

    public async Task<Result> CreateAsync(ItemData data)
    {
        var check = await ValidateAsync(data, null);
        if (!check.Succeeded) return check;

        var item = new Item();
        Apply(item, data);
        await items.AddAsync(item);
        return Result.Ok();
    }

    public async Task<Result> UpdateAsync(int id, ItemData data)
    {
        var item = await items.GetByIdAsync(id);
        if (item is null) return Result.NotFound();

        var check = await ValidateAsync(data, id);
        if (!check.Succeeded) return check;

        Apply(item, data);
        await items.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result> DeleteAsync(int id)
    {
        var item = await items.GetByIdAsync(id);
        if (item is null) return Result.NotFound();

        // Business rule: do not break recipes and the history of user requests
        if (await items.HasDependenciesAsync(id))
            return Result.Fail(string.Empty,
                "This item cannot be deleted: it is used in recipes or has user actions.");

        await items.DeleteAsync(item);
        return Result.Ok();
    }

    private async Task<Result> ValidateAsync(ItemData d, int? excludeId)
    {
        if (string.IsNullOrWhiteSpace(d.InternalName))
            return Result.Fail(nameof(ItemData.InternalName), "Internal name is required.");
        if (string.IsNullOrWhiteSpace(d.Name))
            return Result.Fail(nameof(ItemData.Name), "Name is required.");
        if (d.StackSize is < 1)
            return Result.Fail(nameof(ItemData.StackSize), "Stack size must be at least 1.");
        if (d.CraftTime is < 0)
            return Result.Fail(nameof(ItemData.CraftTime), "Craft time cannot be negative.");
        if (d.ProductAmount is < 1)
            return Result.Fail(nameof(ItemData.ProductAmount), "Product amount must be at least 1.");

        if (!string.IsNullOrWhiteSpace(d.Category) && !ItemCategories.All.Contains(d.Category.Trim()))
            return Result.Fail(nameof(ItemData.Category), "Choose a category from the list.");

        if (!string.IsNullOrWhiteSpace(d.Stats))
        {
            try { JsonDocument.Parse(d.Stats); }
            catch (JsonException)
            {
                return Result.Fail(nameof(ItemData.Stats), "Stats must be valid JSON.");
            }
        }

        if (await items.InternalNameExistsAsync(d.InternalName.Trim().ToLowerInvariant(), excludeId))
            return Result.Fail(nameof(ItemData.InternalName), "An item with this internal name already exists.");

        if (d.UserId is not null && await users.GetByIdAsync(d.UserId.Value) is null)
            return Result.Fail(nameof(ItemData.UserId), "Selected user does not exist.");

        return Result.Ok();
    }

    private static void Apply(Item item, ItemData d)
    {
        item.InternalName = d.InternalName.Trim().ToLowerInvariant();
        item.Name = d.Name.Trim();
        item.Category = Clean(d.Category);
        item.Description = Clean(d.Description);
        item.StackSize = d.StackSize;
        item.CraftTime = d.CraftTime;
        item.ProductAmount = d.ProductAmount;
        item.Stats = Clean(d.Stats);
        item.UserId = d.UserId;
    }

    private static string? Clean(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
