using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AsistenteIA.Services;
using AsistenteIA.Models;

namespace AsistenteIA.ViewModels;

public partial class ChatViewModel : ObservableObject
{
    private readonly AiService _aiService;
    private readonly DatabaseService _dbService;
    private CancellationTokenSource? _cts;

    [ObservableProperty] private string prompt = string.Empty;
    [ObservableProperty] private string answer = string.Empty;
    [ObservableProperty] private string statusMessage = string.Empty;
    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private string selectedModel = "openai/gpt-5.4-nano";

    public ChatViewModel(AiService aiService, DatabaseService dbService)
    {
        _aiService = aiService;
        _dbService = dbService;
    }

    [RelayCommand]
    private async Task AskAsync()
    {
        if (IsBusy) return;

        if (string.IsNullOrWhiteSpace(Prompt))
        {
            StatusMessage = "Por favor ingrese una consulta.";
            return;
        }

        _cts = new CancellationTokenSource();
        IsBusy = true;
        StatusMessage = "Consultando a Gemini...";

        try
        {
            string activeModel = Preferences.Get("SelectedModel", "gemini-3.5-flash-lite");

            Answer = await _aiService.AskAsync(Prompt, activeModel, _cts.Token);

            try
            {
                var record = new ConversationRecord
                {
                    Prompt = Prompt,
                    Answer = Answer,
                    CreatedAt = DateTime.UtcNow,
                    Category = "General",
                    IsVerified = false
                };

                await _dbService.SaveAsync(record);
                StatusMessage = "Respuesta recibida y guardada en el historial.";
            }
            catch (Exception dbEx)
            {
                StatusMessage = $"Respuesta recibida, pero falló guardado local: {dbEx.Message}";
            }
        }
        catch (OperationCanceledException)
        {
            StatusMessage = "Consulta cancelada.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error del servicio: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }


    [RelayCommand]
    private void Cancel() => _cts?.Cancel();
}