using Factorio.Application.Abstractions;
using Factorio.Application.Common;
using Factorio.Domain.Entities;
using Factorio.Domain.Enums;

namespace Factorio.Application.Services;

/// <summary>Use cases for user actions: CRUD plus the main business rule, the admin review.</summary>
public class ActionService(IActionRepository actions, IItemRepository items, IUserRepository users)
{
    private const int MaxDetailsLength = 2000;

    public Task<List<UserAction>> GetAllAsync(ActionStatus? status = null) => actions.GetAllAsync(status);

    public Task<UserAction?> GetAsync(int id) => actions.GetByIdAsync(id);

    public async Task<Result> CreateAsync(ActionData data)
    {
        var check = await ValidateAsync(data);
        if (!check.Succeeded) return check;

        var action = new UserAction { Status = ActionStatus.Pending, CreatedAt = DateTime.UtcNow };
        Apply(action, data);
        await actions.AddAsync(action);
        return Result.Ok();
    }

    public async Task<Result> UpdateAsync(int id, ActionData data)
    {
        var action = await actions.GetByIdAsync(id);
        if (action is null) return Result.NotFound();

        if (action.Status != ActionStatus.Pending)
            return Result.Fail(string.Empty, "Only pending actions can be edited.");

        var check = await ValidateAsync(data);
        if (!check.Succeeded) return check;

        Apply(action, data);
        await actions.SaveChangesAsync();
        return Result.Ok();
    }

    /// <summary>
    /// Admin decision. Approve: Add/Edit write the proposed text into the item description,
    /// Delete clears it, and the item remembers who made the change (Item.UserId).
    /// </summary>
    public async Task<Result> ReviewAsync(int id, bool approve, string? comment)
    {
        var action = await actions.GetByIdAsync(id);
        if (action is null || action.Item is null) return Result.NotFound();

        if (action.Status != ActionStatus.Pending)
            return Result.Fail(string.Empty, "This action has already been reviewed.");

        if (!approve && string.IsNullOrWhiteSpace(comment))
            return Result.Fail(string.Empty, "Please write a comment when rejecting.");

        if (approve)
        {
            action.Item.Description = action.Type == ActionType.Delete
                ? null
                : action.ActionDetails?.Trim();
            action.Item.UserId = action.UserId;
            action.Status = ActionStatus.Approved;
        }
        else
        {
            action.Status = ActionStatus.Rejected;
        }

        action.AdminComment = string.IsNullOrWhiteSpace(comment) ? null : comment.Trim();
        await actions.SaveChangesAsync();   // saves the action and the item in one transaction
        return Result.Ok();
    }

    public async Task<Result> DeleteAsync(int id)
    {
        var action = await actions.GetByIdAsync(id);
        if (action is null) return Result.NotFound();

        await actions.DeleteAsync(action);
        return Result.Ok();
    }

    private async Task<Result> ValidateAsync(ActionData d)
    {
        if (await users.GetByIdAsync(d.UserId) is null)
            return Result.Fail(nameof(ActionData.UserId), "Selected user does not exist.");
        if (await items.GetByIdAsync(d.ItemId) is null)
            return Result.Fail(nameof(ActionData.ItemId), "Selected item does not exist.");

        if (d.Type != ActionType.Delete && string.IsNullOrWhiteSpace(d.Details))
            return Result.Fail(nameof(ActionData.Details), "Proposed description is required for Add and Edit.");
        if (d.Details is { Length: > MaxDetailsLength })
            return Result.Fail(nameof(ActionData.Details), $"Description is too long (max {MaxDetailsLength}).");

        return Result.Ok();
    }

    private static void Apply(UserAction action, ActionData d)
    {
        action.UserId = d.UserId;
        action.ItemId = d.ItemId;
        action.Type = d.Type;
        action.ActionDetails = d.Type == ActionType.Delete || string.IsNullOrWhiteSpace(d.Details)
            ? null
            : d.Details.Trim();
    }
}
