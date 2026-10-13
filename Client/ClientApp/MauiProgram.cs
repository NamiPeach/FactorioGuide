using ClientApp.Services;
using ClientApp.ViewModels;
using ClientApp.Views;
using Microsoft.Extensions.Logging;

namespace ClientApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Services: one instance for the whole app (holds the session in task 9)
        builder.Services.AddSingleton<ITokenStore, PreferencesTokenStore>();
        builder.Services.AddSingleton(new HttpClient { BaseAddress = new Uri(ApiConfig.BaseUrl), Timeout = TimeSpan.FromSeconds(15) });
        builder.Services.AddSingleton<IApiService, ApiService>();
        builder.Services.AddSingleton<INavigationService, ShellNavigationService>();

        // ViewModels and pages: a new instance each time
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<ItemsViewModel>();
        builder.Services.AddTransient<ActionViewModel>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<ItemsPage>();
        builder.Services.AddTransient<ActionPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
