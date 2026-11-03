using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AsistenteIA.Models;
using AsistenteIA.Services;
using AsistenteIA.Views;

namespace AsistenteIA.ViewModels;

/// <summary>Lista de proyectos del estudiante.</summary>
public partial class ProjectsViewModel : ObservableObject
{
    private readonly DatabaseService _dbService;
    private readonly INavigationService _navigation;
    private readonly IDialogService _dialogs;

    [ObservableProperty] private ObservableCollection<Project> items = [];
    [ObservableProperty] private bool isRefreshing;

    public ProjectsViewModel(DatabaseService dbService, INavigationService navigation, IDialogService dialogs)
    {
        _dbService = dbService;
        _navigation = navigation;
        _dialogs = dialogs;
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        IsRefreshing = true;
        try
        {
            Items = new ObservableCollection<Project>(await _dbService.GetProjectsAsync());
        }
        catch (Exception ex)
        {
            await _dialogs.AlertAsync("Error", $"No se pudieron cargar los proyectos: {ex.Message}");
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    private Task NewProjectAsync() => _navigation.GoToAsync(nameof(ProjectDetailPage));

    [RelayCommand]
    private Task OpenProjectAsync(Project? project) =>
        project is null
            ? Task.CompletedTask
            : _navigation.GoToAsync(nameof(ProjectDetailPage),
                new Dictionary<string, object> { ["Project"] = project });
}
