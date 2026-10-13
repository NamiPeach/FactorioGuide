using System.Text.RegularExpressions;

namespace Factorio.Application.Locale;

/// <summary>Parsed Factorio locale file (base.cfg): internal name -> display text.</summary>
public class FactorioLocale
{
    private readonly Dictionary<string, Dictionary<string, string>> _sections = new();

    /// <summary>Parses the ini-like lines of base.cfg ([section] headers and key=value pairs).</summary>
    public static FactorioLocale Parse(IEnumerable<string> lines)
    {
        var locale = new FactorioLocale();
        Dictionary<string, string>? current = null;

        foreach (var raw in lines)
        {
            var line = raw.Trim();
            if (line.Length == 0 || line[0] == ';' || line[0] == '#') continue;

            if (line[0] == '[' && line[^1] == ']')
            {
                var section = line[1..^1];
                if (!locale._sections.TryGetValue(section, out current))
                    locale._sections[section] = current = new Dictionary<string, string>();
                continue;
            }

            var eq = line.IndexOf('=');
            if (current is null || eq <= 0) continue;
            current[line[..eq].Trim()] = line[(eq + 1)..].Trim();
        }
        return locale;
    }

    /// <summary>Display name: items first, then buildings (entities), then fluids.</summary>
    public string? FindName(string key) =>
        Find(key, "item-name", "entity-name", "fluid-name");

    public string? FindDescription(string key) =>
        Find(key, "item-description", "entity-description");

    private string? Find(string key, params string[] sections)
    {
        foreach (var section in sections)
        {
            if (_sections.TryGetValue(section, out var map)
                && map.TryGetValue(key, out var value))
            {
                var cleaned = Clean(value);
                if (cleaned is not null) return cleaned;
            }
        }
        return null;
    }

    /// <summary>Removes game markup: templates, __CONTROL__x__ tags, escaped line breaks.</summary>
    private static string? Clean(string text)
    {
        if (text.Contains("__1__")) return null; // template like "__1__ barrel", not a real name

        text = Regex.Replace(text, @"\\+n", "\n");
        text = Regex.Replace(text, @"__[A-Z_]+__(.+?)__", "$1");
        text = text.Trim();
        return text.Length == 0 ? null : text;
    }
}
