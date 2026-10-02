using Factorio.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Factorio.Infrastructure.Data;

/// <summary>EF Core database context: the tables of the FactorioGuide database.</summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Item> Items => Set<Item>();
    public DbSet<RecipeIngredient> RecipeIngredients => Set<RecipeIngredient>();

    /// <summary>Table "Actions": user requests to change item descriptions.</summary>
    public DbSet<UserAction> Actions => Set<UserAction>();
    public DbSet<Blueprint> Blueprints => Set<Blueprint>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Picks up every IEntityTypeConfiguration in the Configurations folder.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
