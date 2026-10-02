namespace Factorio.Domain.Entities;

/// <summary>One line of a recipe: <see cref="Item"/> is crafted from <see cref="IngredientItem"/> x <see cref="Amount"/>.</summary>
public class RecipeIngredient
{
    public int Id { get; set; }

    public int ItemId { get; set; }
    public Item? Item { get; set; }

    public int IngredientItemId { get; set; }
    public Item? IngredientItem { get; set; }

    public int Amount { get; set; }
}
