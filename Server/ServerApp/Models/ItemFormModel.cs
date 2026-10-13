using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ServerApp.Models;

/// <summary>Form model for creating and editing an item.</summary>
public class ItemFormModel
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    [Display(Name = "Internal name")]
    public string InternalName { get; set; } = string.Empty;

    [Required, StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [StringLength(100)]
    public string? Category { get; set; }

    [StringLength(2000)]
    public string? Description { get; set; }

    [Display(Name = "Stack size")]
    public int? StackSize { get; set; }

    [Display(Name = "Craft time (s)")]
    public double? CraftTime { get; set; }

    [Display(Name = "Product amount")]
    public int? ProductAmount { get; set; }

    [Display(Name = "Stats (JSON)")]
    public string? Stats { get; set; }

    [Display(Name = "Last edited by")]
    public int? UserId { get; set; }

    /// <summary>Options for the user drop-down (filled by the controller, not posted).</summary>
    public List<SelectListItem> UserOptions { get; set; } = new();

    /// <summary>Options for the category drop-down (filled by the controller, not posted).</summary>
    public List<SelectListItem> CategoryOptions { get; set; } = new();
}
