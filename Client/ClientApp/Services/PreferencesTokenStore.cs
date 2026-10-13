namespace ClientApp.Services;

/// <summary>Adapter: ITokenStore implemented with MAUI Preferences.</summary>
public class PreferencesTokenStore : ITokenStore
{
    private const string TokenKey = "access_token";
    private const string ExpiresKey = "access_token_expires";

    public string? GetToken()
    {
        var token = Preferences.Default.Get(TokenKey, string.Empty);
        var expiresTicks = Preferences.Default.Get(ExpiresKey, 0L);

        // One minute of margin so we do not send a token that expires on the way
        if (token.Length == 0 || expiresTicks <= DateTime.UtcNow.AddMinutes(1).Ticks)
            return null;

        return token;
    }

    public void Save(string token, DateTime expiresAtUtc)
    {
        Preferences.Default.Set(TokenKey, token);
        Preferences.Default.Set(ExpiresKey, expiresAtUtc.Ticks);
    }

    public void Clear()
    {
        Preferences.Default.Remove(TokenKey);
        Preferences.Default.Remove(ExpiresKey);
    }
}
