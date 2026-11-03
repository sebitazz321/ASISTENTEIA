using AsistenteIA.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AsistenteIA.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly ApiKeyService _keyService;
    private readonly AiService _aiService;
    private readonly IDialogService _dialogs;

    // Modelos sugeridos (el identificador sigue siendo editable porque el proveedor los cambia).
    public IReadOnlyList<string> SuggestedModels { get; } =
        ["gemini-3.5-flash-lite", "gemini-3.1-flash-lite", "gemini-3.6-flash", "gemini-3.8-flash"];

    // La clave NUNCA se vuelve a mostrar: este campo solo sirve para escribir una nueva.
    [ObservableProperty] private string apiKey = string.Empty;
    [ObservableProperty] private string selectedModel = AiService.GetSavedModel();
    [ObservableProperty] private string statusMessage = string.Empty;
    [ObservableProperty] private string keyStatus = "Comprobando clave...";
    [ObservableProperty] private bool isBusy;

    public SettingsViewModel(ApiKeyService keyService, AiService aiService, IDialogService dialogs)
    {
        _keyService = keyService;
        _aiService = aiService;
        _dialogs = dialogs;
        _ = RefreshKeyStatusAsync();
    }

    private async Task RefreshKeyStatusAsync()
    {
        try
        {
            KeyStatus = await _keyService.HasKeyAsync()
                ? "Clave guardada de forma segura (almacén cifrado del dispositivo)"
                : "Sin clave configurada";
        }
        catch
        {
            KeyStatus = "No se pudo leer el almacén seguro";
        }
    }

    [RelayCommand]
    private async Task SaveKeyAsync()
    {
        string model = SelectedModel?.Trim() ?? string.Empty;
        if (model.Length == 0)
        {
            StatusMessage = "Indique el identificador del modelo.";
            return;
        }

        try
        {
            if (!string.IsNullOrWhiteSpace(ApiKey))
            {
                await _keyService.SaveAsync(ApiKey);
                ApiKey = string.Empty;
            }
            else if (!await _keyService.HasKeyAsync())
            {
                StatusMessage = "Ingrese una clave válida.";
                return;
            }

            Preferences.Set(AiService.ModelPreferenceKey, model);
            await RefreshKeyStatusAsync();
            StatusMessage = "Configuración guardada correctamente.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"No se pudo guardar: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task TestConnectionAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        StatusMessage = "Probando conexión con el proveedor...";

        try
        {
            // Si el usuario escribió una clave nueva, se guarda antes de probar.
            if (!string.IsNullOrWhiteSpace(ApiKey))
            {
                await _keyService.SaveAsync(ApiKey);
                ApiKey = string.Empty;
                await RefreshKeyStatusAsync();
            }

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            string response = await _aiService.AskAsync(
                "Responde con una frase corta: conexión exitosa.", SelectedModel, cts.Token);
            StatusMessage = $"✔ Conexión exitosa. Respuesta: {response}";
        }
        catch (Exception ex)
        {
            StatusMessage = $"✘ {AiService.GetFriendlyMessage(ex)}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task RemoveKeyAsync()
    {
        bool confirm = await _dialogs.ConfirmAsync(
            "Confirmar", "¿Desea eliminar la clave guardada?");
        if (!confirm) return;

        _keyService.Remove();
        ApiKey = string.Empty;
        await RefreshKeyStatusAsync();
        StatusMessage = "Clave eliminada correctamente.";
    }
}
