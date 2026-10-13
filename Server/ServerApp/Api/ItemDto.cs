namespace ServerApp.Api;

/// <summary>An item as returned by the REST API.</summary>
public record ItemDto(
    int Id,
    string InternalName,
    string Name,
    string? Category,
    string? Description,
    int? StackSize,
    double? CraftTime,
    int? ProductAmount);
