using System.Collections.ObjectModel;
using AsistenteIA.Models;
using AsistenteIA.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AsistenteIA.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly ApiKeyService _keyService;
    private readonly AiService _aiService;
    private readonly IDialogService _dialogs;

    /// <summary>Proveedores disponibles (cambio manual: el usuario elige uno y pulsa Guardar).</summary>
    public IReadOnlyList<AiProviderInfo> Providers { get; } = AiProviderCatalog.All;

    // Modelos sugeridos del proveedor elegido (el identificador sigue siendo editable).
    public ObservableCollection<string> SuggestedModels { get; } = [];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ProviderDescription))]
    [NotifyPropertyChangedFor(nameof(KeyPlaceholder))]
    private AiProviderInfo selectedProvider = AiService.CurrentProviderInfo;

    // La clave NUNCA se vuelve a mostrar: este campo solo sirve para escribir una nueva.
    [ObservableProperty] private string apiKey = string.Empty;
    [ObservableProperty] private string selectedModel = AiService.GetSavedModel();
    [ObservableProperty] private string statusMessage = string.Empty;
    [ObservableProperty] private string keyStatus = "Comprobando clave...";
    [ObservableProperty] private string activeProviderText = string.Empty;
    [ObservableProperty] private bool isBusy;

    public string ProviderDescription =>
        $"{SelectedProvider.Description} Obtener clave: {SelectedProvider.KeyHelpUrl}";

    public string KeyPlaceholder => $"Escriba aquí una clave nueva de {SelectedProvider.DisplayName}";

    public SettingsViewModel(ApiKeyService keyService, AiService aiService, IDialogService dialogs)
    {
        _keyService = keyService;
        _aiService = aiService;
        _dialogs = dialogs;
        LoadSuggestedModels(SelectedProvider);
        UpdateActiveProviderText();
        _ = RefreshKeyStatusAsync();
    }

    // Al cambiar de proveedor en el selector se cargan sus modelos y el estado de su clave.
    // El cambio se aplica a las consultas solo al pulsar «Guardar».
    partial void OnSelectedProviderChanged(AiProviderInfo value)
    {
        if (value is null) return;
        ApiKey = string.Empty;
        LoadSuggestedModels(value);
        SelectedModel = AiService.GetSavedModel(value.Id);
        StatusMessage = value.Id == AiService.GetSelectedProvider()
            ? string.Empty
            : $"Pulse «Guardar» para usar {value.DisplayName} en las consultas.";
        _ = RefreshKeyStatusAsync();
    }

    private void LoadSuggestedModels(AiProviderInfo provider)
    {
        SuggestedModels.Clear();
        foreach (string m in provider.SuggestedModels)
            SuggestedModels.Add(m);
    }

    private void UpdateActiveProviderText() =>
        ActiveProviderText = $"Proveedor activo: {AiService.CurrentProviderInfo.DisplayName} · " +
                             $"modelo {AiService.GetSavedModel()}";

    private async Task RefreshKeyStatusAsync()
    {
        try
        {
            KeyStatus = await _keyService.HasKeyAsync(SelectedProvider.Id)
                ? $"Clave de {SelectedProvider.DisplayName} guardada de forma segura (almacén cifrado del dispositivo)"
                : $"Sin clave de {SelectedProvider.DisplayName} configurada";
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
            AiProvider provider = SelectedProvider.Id;
            if (!string.IsNullOrWhiteSpace(ApiKey))
            {
                await _keyService.SaveAsync(ApiKey, provider);
                ApiKey = string.Empty;
            }
            else if (SelectedProvider.KeyRequired && !await _keyService.HasKeyAsync(provider))
            {
                StatusMessage = $"Ingrese una clave válida de {SelectedProvider.DisplayName}.";
                return;
            }

            AiService.SaveModel(provider, model);
            AiService.SetSelectedProvider(provider);
            UpdateActiveProviderText();
            await RefreshKeyStatusAsync();
            StatusMessage = $"Configuración guardada. Las consultas usarán {SelectedProvider.DisplayName}.";
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
        AiProviderInfo provider = SelectedProvider;
        StatusMessage = $"Probando conexión con {provider.DisplayName}...";

        try
        {
            // Si el usuario escribió una clave nueva, se guarda antes de probar.
            if (!string.IsNullOrWhiteSpace(ApiKey))
            {
                await _keyService.SaveAsync(ApiKey, provider.Id);
                ApiKey = string.Empty;
                await RefreshKeyStatusAsync();
            }

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            string response = await _aiService.AskAsync(
                "Responde con una frase corta: conexión exitosa.", SelectedModel, cts.Token,
                projectContext: null, provider: provider.Id);
            StatusMessage = $"✔ Conexión exitosa con {provider.DisplayName}. Respuesta: {response}";
        }
        catch (Exception ex)
        {
            StatusMessage = $"✘ {AiErrorMapper.GetFriendlyMessage(ex, false, provider.DisplayName)}";
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
            "Confirmar", $"¿Desea eliminar la clave guardada de {SelectedProvider.DisplayName}?");
        if (!confirm) return;

        _keyService.Remove(SelectedProvider.Id);
        ApiKey = string.Empty;
        await RefreshKeyStatusAsync();
        StatusMessage = "Clave eliminada correctamente.";
    }
}
