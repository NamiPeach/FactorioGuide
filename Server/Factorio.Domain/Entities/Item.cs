namespace Factorio.Domain.Entities;

/// <summary>A Factorio item or building shown in the wiki.</summary>
public class Item
{
    public int Id { get; set; }

    /// <summary>Internal game name, e.g. "electronic-circuit".</summary>
    public string InternalName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Category { get; set; }
    public string? Description { get; set; }
    public int? StackSize { get; set; }
    public double? CraftTime { get; set; }

    /// <summary>How many units one craft produces.</summary>
    public int? ProductAmount { get; set; }

    /// <summary>Other characteristics as JSON text.</summary>
    public string? Stats { get; set; }

    /// <summary>User whose approved edit changed the description last (null for imported items).</summary>
    public int? UserId { get; set; }
    public User? User { get; set; }

    /// <summary>Ingredients needed to craft this item.</summary>
    public ICollection<RecipeIngredient> Ingredients { get; set; } = new List<RecipeIngredient>();

    /// <summary>Recipes where this item is an ingredient.</summary>
    public ICollection<RecipeIngredient> UsedIn { get; set; } = new List<RecipeIngredient>();

    public ICollection<UserAction> Actions { get; set; } = new List<UserAction>();
}
