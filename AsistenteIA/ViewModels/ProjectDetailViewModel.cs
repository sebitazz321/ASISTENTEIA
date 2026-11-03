using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AsistenteIA.Models;
using AsistenteIA.Services;
using AsistenteIA.Views;

namespace AsistenteIA.ViewModels;

/// <summary>Crear, editar o eliminar un proyecto y ver sus consultas.</summary>
[QueryProperty(nameof(CurrentProject), "Project")]
public partial class ProjectDetailViewModel : ObservableObject
{
    private readonly DatabaseService _dbService;
    private readonly INavigationService _navigation;
    private readonly IDialogService _dialogs;

    [ObservableProperty] private Project? currentProject;
    [ObservableProperty] private string name = string.Empty;
    [ObservableProperty] private string description = string.Empty;
    [ObservableProperty] private string statusMessage = string.Empty;
    [ObservableProperty] private string pageTitle = "Nuevo proyecto";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsExisting))]
    private bool isNew = true;

    public bool IsExisting => !IsNew;
    public ObservableCollection<ConversationRecord> Conversations { get; } = [];

    public int NameMaxLength => Project.NameMaxLength;
    public int DescriptionMaxLength => Project.DescriptionMaxLength;

    public ProjectDetailViewModel(DatabaseService dbService, INavigationService navigation, IDialogService dialogs)
    {
        _dbService = dbService;
        _navigation = navigation;
        _dialogs = dialogs;
    }

    partial void OnCurrentProjectChanged(Project? value)
    {
        if (value is null) return;
        Name = value.Name;
        Description = value.Description;
        IsNew = false;
        PageTitle = "Proyecto";
        _ = LoadConversationsAsync();
    }

    private async Task LoadConversationsAsync()
    {
        if (CurrentProject is null) return;
        try
        {
            List<ConversationRecord> list = await _dbService.GetItemsByProjectAsync(CurrentProject.Id);
            Conversations.Clear();
            foreach (ConversationRecord r in list)
            {
                r.ProjectName = CurrentProject.Name;
                Conversations.Add(r);
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"No se pudieron cargar las consultas: {ex.Message}";
        }
    }

    // CREATE (proyecto nuevo) o UPDATE (proyecto existente).
    [RelayCommand]
    private async Task SaveAsync()
    {
        string cleanName = Name?.Trim() ?? string.Empty;
        string cleanDescription = Description?.Trim() ?? string.Empty;

        if (cleanName.Length < Project.NameMinLength || cleanName.Length > Project.NameMaxLength)
        {
            StatusMessage = $"El nombre debe tener entre {Project.NameMinLength} y {Project.NameMaxLength} caracteres.";
            return;
        }
        if (cleanDescription.Length > Project.DescriptionMaxLength)
        {
            StatusMessage = $"La descripción supera {Project.DescriptionMaxLength} caracteres.";
            return;
        }

        try
        {
            bool wasNew = IsNew;
            Project project = CurrentProject ?? new Project();
            project.Name = cleanName;
            project.Description = cleanDescription;
            await _dbService.SaveProjectAsync(project);

            CurrentProject = project;
            StatusMessage = wasNew ? "Proyecto creado." : "Proyecto actualizado.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"No se pudo guardar: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        if (CurrentProject is null) return;

        bool confirm = await _dialogs.ConfirmAsync(
            "Eliminar proyecto",
            "Se eliminará el proyecto. Sus consultas se conservarán en el historial sin proyecto. ¿Continuar?");
        if (!confirm) return;

        await _dbService.DeleteProjectAsync(CurrentProject);
        await _navigation.GoBackAsync();
    }

    [RelayCommand]
    private Task StartConsultationAsync() =>
        CurrentProject is null
            ? Task.CompletedTask
            : _navigation.GoToAsync($"//{nameof(ChatPage)}?projectId={CurrentProject.Id}");

    [RelayCommand]
    private Task OpenConversationAsync(ConversationRecord? record) =>
        record is null
            ? Task.CompletedTask
            : _navigation.GoToAsync(nameof(DetailPage),
                new Dictionary<string, object> { ["Record"] = record });
}
