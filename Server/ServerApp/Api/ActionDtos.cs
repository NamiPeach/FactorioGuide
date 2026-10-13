using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Factorio.Domain.Enums;

namespace ServerApp.Api;

/// <summary>Body of POST /api/actions. The user is taken from the access token.</summary>
public class CreateActionRequest
{
    [Required, Range(1, int.MaxValue)]
    public int? ItemId { get; set; }

    /// <summary>Add, Edit or Delete.</summary>
    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ActionType? Type { get; set; }

    /// <summary>Proposed description (required for Add and Edit, ignored for Delete).</summary>
    [StringLength(2000)]
    public string? Details { get; set; }
}

/// <summary>An action as returned by the REST API.</summary>
public record ActionDto(
    int Id,
    int UserId,
    int ItemId,
    [property: JsonConverter(typeof(JsonStringEnumConverter))] ActionType Type,
    [property: JsonConverter(typeof(JsonStringEnumConverter))] ActionStatus Status,
    string? Details,
    string? AdminComment,
    DateTime CreatedAt);

/// <summary>Response of POST /api/actions: the request is stored and waits for admin review.</summary>
public record ActionAcceptedDto(string Status, string Message);
