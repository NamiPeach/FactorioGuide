using System.Collections.ObjectModel;
using ClientApp.Models;
using ClientApp.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ClientApp.ViewModels;

public partial class ItemsViewModel(IApiService api, INavigationService navigation) : ViewModelBase
{
    // Cache: the list is loaded once and reused until the user presses Refresh
    private List<ItemModel> _all = new();

    public ObservableCollection<ItemModel> Items { get; } = new();

    [ObservableProperty] private string searchText = string.Empty;

    partial void OnSearchTextChanged(string value) => ApplyFilter();

    [RelayCommand]
    private async Task LoadAsync()
    {
        if (_all.Count > 0) return;
        await FetchAsync();
    }

    [RelayCommand]
    private Task RefreshAsync() => FetchAsync();

    [RelayCommand]
    private Task ProposeChangeAsync(ItemModel item) =>
        navigation.GoToAsync($"action?itemId={item.Id}&itemName={Uri.EscapeDataString(item.Name)}");

    [RelayCommand]
    private async Task LogoutAsync()
    {
        api.Logout();
        _all = new List<ItemModel>();
        Items.Clear();
        await navigation.GoToAsync("//login");
    }

    private async Task FetchAsync()
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;
            ErrorMessage = null;
            _all = (await api.GetItemsAsync()).ToList();
            ApplyFilter();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Could not load items: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void ApplyFilter()
    {
        var query = SearchText.Trim();
        var source = query.Length == 0
            ? _all
            : _all.Where(i => i.Name.Contains(query, StringComparison.OrdinalIgnoreCase)
                              || i.InternalName.Contains(query, StringComparison.OrdinalIgnoreCase));

        Items.Clear();
        foreach (var item in source) Items.Add(item);
    }
}
