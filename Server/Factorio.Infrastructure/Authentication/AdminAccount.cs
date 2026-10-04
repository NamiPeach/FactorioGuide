using System.Security.Cryptography;
using System.Text;

namespace Factorio.Infrastructure.Authentication;

/// <summary>
/// The single admin account of the admin panel.
/// The credentials are defined in code (assignment requirement) and are demo values only.
/// </summary>
public static class AdminAccount
{
    public const string Login = "admin";
    public const string Password = "admin123";

    /// <summary>Checks the entered login and password (comparison takes constant time).</summary>
    public static bool IsValid(string? login, string? password)
    {
        var loginOk = CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(login ?? string.Empty),
            Encoding.UTF8.GetBytes(Login));

        var passwordOk = CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(password ?? string.Empty),
            Encoding.UTF8.GetBytes(Password));

        return loginOk & passwordOk;
    }
}