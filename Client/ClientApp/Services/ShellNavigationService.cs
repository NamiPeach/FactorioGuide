namespace ClientApp.Services;

/// <summary>Adapter: INavigationService implemented with Shell.</summary>
public class ShellNavigationService : INavigationService
{
    public Task GoToAsync(string route) => Shell.Current.GoToAsync(route);

    public Task AlertAsync(string title, string message) =>
        Shell.Current.DisplayAlertAsync(title, message, "OK");
}
