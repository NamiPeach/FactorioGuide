using Factorio.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Factorio.Infrastructure.Data.Configurations;

public class ItemConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.ToTable("Items");
        builder.HasKey(i => i.Id);
        builder.Property(i => i.InternalName).IsRequired().HasMaxLength(100);
        builder.Property(i => i.Name).IsRequired().HasMaxLength(100);
        builder.Property(i => i.Category).HasMaxLength(100);
        builder.HasIndex(i => i.InternalName).IsUnique();

        // Items.UserId -> Users.Id (optional: the user whose approved edit changed the description)
        builder.HasOne(i => i.User)
            .WithMany(u => u.Items)
            .HasForeignKey(i => i.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        // Test data
        builder.HasData(
            new Item
            {
                Id = 1, InternalName = "iron-plate", Name = "Iron plate", Category = "Intermediate products",
                Description = "Basic building material made by smelting iron ore.",
                StackSize = 100, CraftTime = 3.2, ProductAmount = 1, UserId = 2
            },
            new Item
            {
                Id = 2, InternalName = "copper-plate", Name = "Copper plate", Category = "Intermediate products",
                StackSize = 100, CraftTime = 3.2, ProductAmount = 1
            },
            new Item
            {
                Id = 3, InternalName = "copper-cable", Name = "Copper cable", Category = "Intermediate products",
                StackSize = 200, CraftTime = 0.5, ProductAmount = 2
            },
            new Item
            {
                Id = 4, InternalName = "electronic-circuit", Name = "Electronic circuit", Category = "Intermediate products",
                StackSize = 200, CraftTime = 0.5, ProductAmount = 1
            });
    }
}
