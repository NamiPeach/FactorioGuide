namespace Factorio.Application.Common;

/// <summary>Fixed list of in-game item groups. The admin panel shows it as a drop-down.</summary>
public static class ItemCategories
{
    public static readonly IReadOnlyList<string> All = new[]
    {
        "Logistics",
        "Production",
        "Intermediate products",
        "Combat",
        "Fluids",
        "Space"
    };
}
