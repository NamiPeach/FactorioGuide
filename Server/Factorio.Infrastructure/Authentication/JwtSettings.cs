namespace Factorio.Infrastructure.Authentication;

/// <summary>JWT settings read from the "Jwt" section of appsettings.json.</summary>
public class JwtSettings
{
    public const string SectionName = "Jwt";

    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpiresMinutes { get; set; } = 


60;

}

