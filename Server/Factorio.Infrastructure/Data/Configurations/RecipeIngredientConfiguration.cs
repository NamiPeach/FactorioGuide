using Factorio.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Factorio.Infrastructure.Data.Configurations;

public class RecipeIngredientConfiguration : IEntityTypeConfiguration<RecipeIngredient>
{
    public void Configure(EntityTypeBuilder<RecipeIngredient> builder)
    {
        builder.ToTable("RecipeIngredients");
        builder.HasKey(r => r.Id);

        // The item that is crafted
        builder.HasOne(r => r.Item)
            .WithMany(i => i.Ingredients)
            .HasForeignKey(r => r.ItemId)
            .OnDelete(DeleteBehavior.Cascade);

        // The item used as an ingredient
        builder.HasOne(r => r.IngredientItem)
            .WithMany(i => i.UsedIn)
            .HasForeignKey(r => r.IngredientItemId)
            .OnDelete(DeleteBehavior.Cascade);

        // Test data: copper cable = copper plate; electronic circuit = iron plate + 3 copper cable
        builder.HasData(
            new RecipeIngredient { Id = 1, ItemId = 3, IngredientItemId = 2, Amount = 1 },
            new RecipeIngredient { Id = 2, ItemId = 4, IngredientItemId = 1, Amount = 1 },
            new RecipeIngredient { Id = 3, ItemId = 4, IngredientItemId = 3, Amount = 3 });
    }
}
