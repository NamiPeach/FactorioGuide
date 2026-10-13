using Factorio.Application.Abstractions;
using Factorio.Application.Locale;

namespace Factorio.Application.Services;

/// <summary>What the import did, for the console report.</summary>
public record LocaleImportResult(int Checked, int NamesUpdated, int DescriptionsSet, List<string> NotFound);

/// <summary>Use case: fill item names and descriptions from the game's locale file.</summary>
public class LocaleImportService(IItemLocaleRepository repository)
{
    public async Task<LocaleImportResult> ImportAsync(IEnumerable<string> localeLines)
    {
        var locale = FactorioLocale.Parse(localeLines);
        var items = await repository.GetImportableItemsAsync();

        int namesUpdated = 0, descriptionsSet = 0;
        var notFound = new List<string>();

        foreach (var item in items)
        {
            var name = locale.FindName(item.InternalName);
            if (name is null)
                notFound.Add(item.InternalName);
            else if (item.Name != name)
            {
                item.Name = name;
                namesUpdated++;
            }

            // Never overwrite an existing description (it may come from an approved user edit)
            var description = locale.FindDescription(item.InternalName);
            if (description is not null && string.IsNullOrWhiteSpace(item.Description))
            {
                item.Description = description;
                descriptionsSet++;
            }
        }

        await repository.SaveChangesAsync();
        return new LocaleImportResult(items.Count, namesUpdated, descriptionsSet, notFound);
    }
}
