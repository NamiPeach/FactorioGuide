namespace ClientApp.Services;

/// <summary>Port: where the JWT is kept between app launches.</summary>
public interface ITokenStore
{
    /// <summary>The saved token, or null if there is none or it is about to expire.</summary>
    string? GetToken();
    void Save(string token, DateTime expiresAtUtc);
    void Clear();
}
