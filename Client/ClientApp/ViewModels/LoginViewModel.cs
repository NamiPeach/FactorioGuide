using ClientApp.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ClientApp.ViewModels;

public partial class LoginViewModel(IApiService api, INavigationService navigation) : ViewModelBase
{
    [ObservableProperty] private string email = string.Empty;
    [ObservableProperty] private string password = string.Empty;

    /// <summary>If a valid token is already saved, skip the login screen.</summary>
    [RelayCommand]
    private async Task CheckSessionAsync()
    {
        if (api.IsLoggedIn)
            await navigation.GoToAsync("//items");
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (IsBusy) return;
        ErrorMessage = null;

        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Enter email and password.";
            return;
        }

        try
        {
            IsBusy = true;
            var result = await api.LoginAsync(Email.Trim(), Password);
            if (!result.Success)
            {
                ErrorMessage = result.Message;
                return;
            }

            Password = string.Empty;
            await navigation.GoToAsync("//items");
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }
}
