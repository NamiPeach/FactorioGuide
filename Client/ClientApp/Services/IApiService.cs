using ClientApp.Models;

namespace ClientApp.Services;

/// <summary>Everything the app needs from the server. ViewModels depend only on this interface.</summary>
public interface IApiService
{
    /// <summary>True if a valid (not expired) token is saved.</summary>
    bool IsLoggedIn { get; }

    Task<ApiResult> LoginAsync(string email, string password);
    Task<IReadOnlyList<ItemModel>> GetItemsAsync();
    Task<ApiResult> CreateActionAsync(int itemId, string type, string? details);
    void Logout();
}
