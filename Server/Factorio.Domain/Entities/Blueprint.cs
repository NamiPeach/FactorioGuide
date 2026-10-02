namespace Factorio.Domain.Entities;

/// <summary>A Factorio blueprint shared by a user (no moderation).</summary>
public class Blueprint
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }

    /// <summary>The blueprint string that can be pasted into the game.</summary>
    public string BlueprintString { get; set; } = string.Empty;

    /// <summary>Path of the optional screenshot file stored on the server.</summary>
    public string? ScreenshotPath { get; set; }

    public int? AuthorId { get; set; }
    public User? Author { get; set; }

    public DateTime CreatedAt { get; set; }
}
