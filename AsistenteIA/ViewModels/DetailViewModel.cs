using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AsistenteIA.Models;
using AsistenteIA.Services;

namespace AsistenteIA.ViewModels;

[QueryProperty(nameof(CurrentRecord), "Record")]
public partial class DetailViewModel : ObservableObject
{
    private readonly DatabaseService _dbService;
    private readonly INavigationService _navigation;
    private readonly IDialogService _dialogs;

    [ObservableProperty] private ConversationRecord? currentRecord;
    [ObservableProperty] private string prompt = string.Empty;
    [ObservableProperty] private string answer = string.Empty;
    [ObservableProperty] private string createdAtText = string.Empty;
    [ObservableProperty] private string category = string.Empty;
    [ObservableProperty] private string projectName = string.Empty;
    [ObservableProperty] private string verificationText = string.Empty;
    [ObservableProperty] private string verifyButtonText = string.Empty;
    [ObservableProperty] private string note = string.Empty;
    [ObservableProperty] private string statusMessage = string.Empty;

    public int NoteMaxLength => ConversationRecord.NoteMaxLength;

    public DetailViewModel(DatabaseService dbService, INavigationService navigation, IDialogService dialogs)
    {
        _dbService = dbService;
        _navigation = navigation;
        _dialogs = dialogs;
    }

    partial void OnCurrentRecordChanged(ConversationRecord? value)
    {
        if (value is null) return;
        Prompt = value.Prompt;
        Answer = value.Answer;
        CreatedAtText = value.CreatedAtLocal;
        Category = value.Category;
        ProjectName = string.IsNullOrEmpty(value.ProjectName) ? "Sin proyecto" : value.ProjectName;
        Note = value.Note;
        Refresh(value);
    }

    private void Refresh(ConversationRecord r)
    {
        VerificationText = r.IsVerified
            ? "✔ Verificada con fuentes confiables"
            : "Sin verificar: contraste la respuesta con fuentes confiables";
        VerifyButtonText = r.IsVerified ? "Quitar verificación" : "Marcar como verificada";
    }

    // Marcar como verificada después de contrastar con fuentes confiables (UPDATE).
    [RelayCommand]
    private async Task ToggleVerifiedAsync()
    {
        if (CurrentRecord is null) return;
        CurrentRecord.IsVerified = !CurrentRecord.IsVerified;
        await _dbService.SaveAsync(CurrentRecord);
        Refresh(CurrentRecord);
    }

    // Editar la nota personal (UPDATE).
    [RelayCommand]
    private async Task SaveNoteAsync()
    {
        if (CurrentRecord is null) return;

        string clean = Note?.Trim() ?? string.Empty;
        if (clean.Length > ConversationRecord.NoteMaxLength)
        {
            StatusMessage = $"La nota supera {ConversationRecord.NoteMaxLength} caracteres.";
            return;
        }

        try
        {
            CurrentRecord.Note = clean;
            await _dbService.SaveAsync(CurrentRecord);
            StatusMessage = "Nota guardada.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"No se pudo guardar la nota: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        if (CurrentRecord is null) return;

        bool confirm = await _dialogs.ConfirmAsync(
            "Confirmar", "¿Desea eliminar esta consulta del historial?");
        if (!confirm) return;

        await _dbService.DeleteAsync(CurrentRecord);
        await _navigation.GoBackAsync();
    }
}
