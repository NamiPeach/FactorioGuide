using Factorio.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Factorio.Infrastructure.Data.Configurations;

public class BlueprintConfiguration : IEntityTypeConfiguration<Blueprint>
{
    public void Configure(EntityTypeBuilder<Blueprint> builder)
    {
        builder.ToTable("Blueprints");
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Title).IsRequired().HasMaxLength(200);
        builder.Property(b => b.BlueprintString).IsRequired();

        // Blueprints.AuthorId -> Users.Id (the blueprint stays if the user is deleted)
        builder.HasOne(b => b.Author)
            .WithMany(u => u.Blueprints)
            .HasForeignKey(b => b.AuthorId)
            .OnDelete(DeleteBehavior.SetNull);

        // Test data
        builder.HasData(new Blueprint
        {
            Id = 1,
            Title = "Sample blueprint",
            Description = "Placeholder entry for testing",
            BlueprintString = "SAMPLE-BLUEPRINT-STRING",
            AuthorId = 1,
            CreatedAt = SeedData.CreatedAt
        });
    }
}
