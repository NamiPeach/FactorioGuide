using System.ComponentModel.DataAnnotations;

namespace ServerApp.Models;

/// <summary>Fields the admin enters when creating or editing a user (never exposes PasswordHash).</summary>
public class UserFormModel
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(200)]
    public string Email { get; set; } = string.Empty;

    /// <summary>Required on create; on edit leave empty to keep the current password.</summary>
    [StringLength(100, MinimumLength = 6)]
    public string? Password { get; set; }
}
