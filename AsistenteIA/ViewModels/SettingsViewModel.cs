using AsistenteIA.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AsistenteIA.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly ApiKeyService _keyService;
    private readonly AiService _aiService;

    [ObservableProperty] private string apiKey = string.Empty;
    [ObservableProperty] private string selectedModel = "gemini-1.5-flash";
    [ObservableProperty] private string statusMessage = string.Empty;
    [ObservableProperty] private bool isBusy;

    public SettingsViewModel(ApiKeyService keyService, AiService aiService)
    {
        _keyService = keyService;
        _aiService = aiService;
        LoadKeyAsync();
    }

    private async void LoadKeyAsync()
    {
        ApiKey = await _keyService.GetAsync() ?? string.Empty;
    }

    [RelayCommand]
    private async Task SaveKeyAsync()
    {
        if (string.IsNullOrWhiteSpace(ApiKey))
        {
            StatusMessage = "Por favor ingrese una clave válida.";
            return;
        }

        await _keyService.SaveAsync(ApiKey);
        Preferences.Set("SelectedModel", SelectedModel);
        StatusMessage = "Clave y modelo guardados correctamente.";
    }

    [RelayCommand]
    private async Task TestConnectionAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        StatusMessage = "Probando conexión con el proveedor...";

        try
        {
            string testPrompt = "Responde 'Conexión exitosa' en una frase corta.";
            string response = await _aiService.AskAsync(testPrompt, SelectedModel);
            StatusMessage = $"Prueba completada: {response}";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error de prueba: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task RemoveKeyAsync()
    {
        bool confirm = await Shell.Current.DisplayAlert(
            "Confirmar", "¿Desea eliminar la clave guardada?", "Sí", "No");

        if (confirm)
        {
            _keyService.Remove();
            ApiKey = string.Empty;
            StatusMessage = "Clave eliminada correctamente.";
        }
    }
}