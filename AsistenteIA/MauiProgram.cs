using Microsoft.Extensions.Logging;
using AsistenteIA.Services;

namespace AsistenteIA;

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

        // --- Registro de Servicios REST y Seguridad ---
        builder.Services.AddSingleton(new HttpClient
        {
            BaseAddress = new Uri("https://generativelanguage.googleapis.com/v1beta/"),
            Timeout = TimeSpan.FromSeconds(60)
        });

        builder.Services.AddSingleton<ApiKeyService>();
        builder.Services.AddSingleton<AiService>();
        builder.Services.AddTransient<AsistenteIA.ViewModels.ChatViewModel>();
        builder.Services.AddTransient<AsistenteIA.Views.ChatPage>();
        builder.Services.AddSingleton<AsistenteIA.Services.DatabaseService>();
        builder.Services.AddTransient<AsistenteIA.ViewModels.HistoryViewModel>();
        builder.Services.AddTransient<AsistenteIA.Views.HistoryPage>();
        builder.Services.AddTransient<AsistenteIA.ViewModels.SettingsViewModel>();
        builder.Services.AddTransient<AsistenteIA.Views.SettingsPage>();



#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}

