namespace Factorio.Infrastructure.Data;

/// <summary>Fixed values shared by the test data (EF HasData needs constants, not DateTime.Now).</summary>
internal static class SeedData
{
    public static readonly DateTime CreatedAt = new(2026, 10, 2, 0, 0, 0, DateTimeKind.Utc);
}
