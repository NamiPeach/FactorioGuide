using System.ComponentModel.DataAnnotations;

namespace ServerApp.Models;

/// <summary>Data entered on the admin login form.</summary>
public class LoginModel
{
    [Required]
    public string Login { get; set; } = string.Empty;

    [Required, DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    /// <summary>Page to return to after a successful login (local URLs only).</summary>
    public string? ReturnUrl { get; set; }
}