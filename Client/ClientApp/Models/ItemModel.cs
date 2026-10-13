namespace ClientApp.Models;

/// <summary>An item as returned by GET /api/items (client-side copy of the server DTO).</summary>
public record ItemModel(
    int Id,
    string InternalName,
    string Name,
    string? Category,
    string? Description,
    int? StackSize,
    double? CraftTime,
    int? ProductAmount);
