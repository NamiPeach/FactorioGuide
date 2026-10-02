using Factorio.Domain.Enums;

namespace Factorio.Domain.Entities;

/// <summary>
/// A user request to add, edit or delete an item description (table "Actions").
/// Named UserAction to avoid a clash with System.Action.
/// </summary>
public class UserAction
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User? User { get; set; }

    public int ItemId { get; set; }
    public Item? Item { get; set; }

    public ActionType Type { get; set; }

    /// <summary>The proposed new description (empty for Delete).</summary>
    public string? ActionDetails { get; set; }

    public ActionStatus Status { get; set; } = ActionStatus.Pending;
    public string? AdminComment { get; set; }
    public DateTime CreatedAt { get; set; }
}
