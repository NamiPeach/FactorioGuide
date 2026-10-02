namespace Factorio.Domain.Entities;

/// <summary>A registered user of the MAUI app. The single admin is defined in code, not here.</summary>
public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    /// <summary>Items whose description was last changed by this user's approved action.</summary>
    public ICollection<Item> Items { get; set; } = new List<Item>();
    public ICollection<UserAction> Actions { get; set; } = new List<UserAction>();
    public ICollection<Blueprint> Blueprints { get; set; } = new List<Blueprint>();
}
