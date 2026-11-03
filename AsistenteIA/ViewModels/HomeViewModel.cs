using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AsistenteIA.Services;
using AsistenteIA.Views;

namespace AsistenteIA.ViewModels;

public partial class HomeViewModel : ObservableObject
{
    private readonly INavigationService _navigation;
    private readonly IDialogService _dialogs;
    private readonly ProfileService _profileService;

    [ObservableProperty] private string greeting = "Bienvenido";

    public HomeViewModel(INavigationService navigation, IDialogService dialogs, ProfileService profileService)
    {
        _navigation = navigation;
        _dialogs = dialogs;
        _profileService = profileService;
    }

    /// <summary>Navega a una pestaña (HomePage, ChatPage, ProjectsPage, HistoryPage, ProfilePage).</summary>
    [RelayCommand]
    private Task GoToAsync(string route) => _navigation.GoToAsync($"//{route}");

    [RelayCommand]
    private Task OpenSettingsAsync() => _navigation.GoToAsync(nameof(SettingsPage));

    public async Task OnAppearingAsync()
    {
        string name = _profileService.Load().Name;
        Greeting = string.IsNullOrWhiteSpace(name)
            ? "Bienvenido"
            : $"Hola, {name.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0]}";

        // Aviso de uso responsable: se muestra una sola vez.
        if (!_profileService.NoticeAccepted)
        {
            try
            {
                await _dialogs.AlertAsync(
                    "Uso responsable de la IA",
                    "• La IA puede equivocarse o inventar datos: verifique la información.\n" +
                    "• No escriba datos personales, contraseñas ni información confidencial.\n" +
                    "• Sus consultas se envían a un proveedor externo para generar la respuesta.\n" +
                    "• Su historial y su clave se guardan solo en este dispositivo.",
                    "Entendido");
                _profileService.NoticeAccepted = true;
            }
            catch (Exception)
            {
                // Si la ventana aún no está lista, el aviso se mostrará la próxima vez.
            }
        }
    }
}
