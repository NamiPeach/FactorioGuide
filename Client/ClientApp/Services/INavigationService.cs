namespace ClientApp.Services;

/// <summary>Navigation and dialogs, so ViewModels do not touch Shell directly.</summary>
public interface INavigationService
{
    Task GoToAsync(string route);
    Task AlertAsync(string title, string message);
}
