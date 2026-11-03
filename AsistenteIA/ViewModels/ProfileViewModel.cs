using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AsistenteIA.Models;
using AsistenteIA.Services;
using AsistenteIA.Views;

namespace AsistenteIA.ViewModels;

/// <summary>Perfil del estudiante, estadísticas de uso y controles de privacidad.</summary>
public partial class ProfileViewModel : ObservableObject
{
    private readonly ProfileService _profileService;
    private readonly DatabaseService _dbService;
    private readonly INavigationService _navigation;
    private readonly IDialogService _dialogs;

    [ObservableProperty] private string name = string.Empty;
    [ObservableProperty] private string program = string.Empty;
    [ObservableProperty] private string institution = string.Empty;
    [ObservableProperty] private string initials = "?";
    [ObservableProperty] private string statusMessage = string.Empty;
    [ObservableProperty] private int totalConsultations;
    [ObservableProperty] private int verifiedConsultations;
    [ObservableProperty] private int projectCount;

    public int FieldMaxLength => UserProfile.FieldMaxLength;

    public ProfileViewModel(ProfileService profileService, DatabaseService dbService,
        INavigationService navigation, IDialogService dialogs)
    {
        _profileService = profileService;
        _dbService = dbService;
        _navigation = navigation;
        _dialogs = dialogs;
    }

    partial void OnNameChanged(string value) =>
        Initials = new UserProfile { Name = value ?? string.Empty }.Initials;

    public async Task LoadAsync()
    {
        UserProfile profile = _profileService.Load();
        Name = profile.Name;
        Program = profile.Program;
        Institution = profile.Institution;

        try
        {
            (int total, int verified, int projects) = await _dbService.GetStatsAsync();
            TotalConsultations = total;
            VerifiedConsultations = verified;
            ProjectCount = projects;
        }
        catch (Exception ex)
        {
            StatusMessage = $"No se pudieron cargar las estadísticas: {ex.Message}";
        }
    }

    [RelayCommand]
    private void SaveProfile()
    {
        string cleanName = Name?.Trim() ?? string.Empty;
        string cleanProgram = Program?.Trim() ?? string.Empty;
        string cleanInstitution = Institution?.Trim() ?? string.Empty;

        if (cleanName.Length < 2)
        {
            StatusMessage = "Escriba su nombre (mínimo 2 caracteres).";
            return;
        }
        if (cleanName.Length > UserProfile.FieldMaxLength ||
            cleanProgram.Length > UserProfile.FieldMaxLength ||
            cleanInstitution.Length > UserProfile.FieldMaxLength)
        {
            StatusMessage = $"Cada campo admite hasta {UserProfile.FieldMaxLength} caracteres.";
            return;
        }

        _profileService.Save(new UserProfile
        {
            Name = cleanName,
            Program = cleanProgram,
            Institution = cleanInstitution
        });
        StatusMessage = "Perfil guardado en este dispositivo.";
    }

    [RelayCommand]
    private Task OpenSettingsAsync() => _navigation.GoToAsync(nameof(SettingsPage));

    [RelayCommand]
    private async Task ClearHistoryAsync()
    {
        bool confirm = await _dialogs.ConfirmAsync(
            "Borrar historial", "Se eliminarán todas las consultas guardadas. Esta acción no se puede deshacer. ¿Continuar?");
        if (!confirm) return;

        await _dbService.DeleteAllItemsAsync();
        await LoadAsync();
        StatusMessage = "Historial eliminado.";
    }

    // Derecho a borrar: elimina consultas, proyectos y perfil del dispositivo.
    [RelayCommand]
    private async Task DeleteAllDataAsync()
    {
        bool confirm = await _dialogs.ConfirmAsync(
            "Borrar todos mis datos",
            "Se eliminarán consultas, proyectos y perfil de este dispositivo. La clave de API se elimina desde Configuración. ¿Continuar?");
        if (!confirm) return;

        await _dbService.DeleteAllItemsAsync();
        await _dbService.DeleteAllProjectsAsync();
        _profileService.Clear();
        await LoadAsync();
        StatusMessage = "Todos los datos locales fueron eliminados.";
    }
}
