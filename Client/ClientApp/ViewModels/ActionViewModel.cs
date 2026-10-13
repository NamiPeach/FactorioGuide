using ClientApp.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ClientApp.ViewModels;

public partial class ActionViewModel(IApiService api, INavigationService navigation)
    : ViewModelBase, IQueryAttributable
{
    public IReadOnlyList<string> Types { get; } = new[] { "Add", "Edit", "Delete" };

    [ObservableProperty] private int itemId;
    [ObservableProperty] private string itemName = string.Empty;
    [ObservableProperty] private string details = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(NeedsDetails))]
    private string selectedType = "Edit";

    /// <summary>Delete does not need a description.</summary>
    public bool NeedsDetails => SelectedType != "Delete";

    /// <summary>Receives itemId and itemName from the Items page.</summary>
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("itemId", out var id) && int.TryParse(id?.ToString(), out var parsed))
            ItemId = parsed;
        if (query.TryGetValue("itemName", out var name))
            ItemName = Uri.UnescapeDataString(name?.ToString() ?? string.Empty);

        SelectedType = "Edit";
        Details = string.Empty;
        ErrorMessage = null;
    }

    [RelayCommand]
    private async Task SubmitAsync()
    {
        if (IsBusy) return;
        ErrorMessage = null;

        var text = Details.Trim();
        if (NeedsDetails && text.Length == 0)
        {
            ErrorMessage = "Description is required for Add and Edit.";
            return;
        }
        if (text.Length > 2000)
        {
            ErrorMessage = "Description is too long (max 2000 characters).";
            return;
        }

        try
        {
            IsBusy = true;
            var result = await api.CreateActionAsync(ItemId, SelectedType, NeedsDetails ? text : null);
            if (!result.Success)
            {
                ErrorMessage = result.Message;
                return;
            }

            await navigation.AlertAsync("Sent", result.Message);
            await navigation.GoToAsync("..");
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Could not send the request: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private Task CancelAsync() => navigation.GoToAsync("..");
}
