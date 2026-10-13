namespace Factorio.Application.Services;

/// <summary>Input for creating or editing an item (keeps use case signatures short).</summary>
public record ItemData(
    string InternalName,
    string Name,
    string? Category,
    string? Description,
    int? StackSize,
    double? CraftTime,
    int? ProductAmount,
    string? Stats,
    int? UserId);
