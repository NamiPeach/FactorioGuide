using Factorio.Domain.Entities;
using Factorio.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Factorio.Infrastructure.Data.Configurations;

public class UserActionConfiguration : IEntityTypeConfiguration<UserAction>
{
    public void Configure(EntityTypeBuilder<UserAction> builder)
    {
        builder.ToTable("Actions");
        builder.HasKey(a => a.Id);

        // Enums are stored as readable text: Add / Edit / Delete and Pending / Approved / Rejected
        builder.Property(a => a.Type).HasConversion<string>().HasMaxLength(10);
        builder.Property(a => a.Status).HasConversion<string>().HasMaxLength(10);

        // Actions.UserId -> Users.Id
        builder.HasOne(a => a.User)
            .WithMany(u => u.Actions)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Actions.ItemId -> Items.Id
        builder.HasOne(a => a.Item)
            .WithMany(i => i.Actions)
            .HasForeignKey(a => a.ItemId)
            .OnDelete(DeleteBehavior.Cascade);

        // Test data
        builder.HasData(
            new UserAction
            {
                Id = 1, UserId = 1, ItemId = 3, Type = ActionType.Add,
                ActionDetails = "Thin wire used in electronic circuits and power poles.",
                Status = ActionStatus.Pending, CreatedAt = SeedData.CreatedAt
            },
            new UserAction
            {
                Id = 2, UserId = 2, ItemId = 1, Type = ActionType.Edit,
                ActionDetails = "Basic building material made by smelting iron ore.",
                Status = ActionStatus.Approved, AdminComment = "Looks good", CreatedAt = SeedData.CreatedAt
            });
    }
}
