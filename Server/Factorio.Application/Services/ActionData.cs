using Factorio.Domain.Enums;

namespace Factorio.Application.Services;

/// <summary>Input for creating or editing an action.</summary>
public record ActionData(int UserId, int ItemId, ActionType Type, string? Details);
