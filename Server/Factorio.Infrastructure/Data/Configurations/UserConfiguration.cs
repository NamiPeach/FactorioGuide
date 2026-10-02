using Factorio.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Factorio.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Name).IsRequired().HasMaxLength(100);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(200);
        builder.Property(u => u.PasswordHash).IsRequired();
        builder.HasIndex(u => u.Email).IsUnique();

        // Test data (password of both users: user123)
        builder.HasData(
            new User
            {
                Id = 1,
                Name = "Test User",
                Email = "user1@test.com",
                PasswordHash = "$2b$11$GIeYTa5Uz6zPLI2AwbkJAOhB6eVEW0P6YEnEV6PgGtO5wTWbeffwW",
                CreatedAt = SeedData.CreatedAt
            },
            new User
            {
                Id = 2,
                Name = "Test User 2",
                Email = "user2@test.com",
                PasswordHash = "$2b$11$IVm8leFGjvB9x4NoS29XhOU4oKZnD.mXxcMDOyptJncJlGlBpwPKm",
                CreatedAt = SeedData.CreatedAt
            });
    }
}
