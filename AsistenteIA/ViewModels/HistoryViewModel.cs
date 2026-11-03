using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AsistenteIA.Models;
using AsistenteIA.Services;
using AsistenteIA.Views;

namespace AsistenteIA.ViewModels;

public partial class HistoryViewModel : ObservableObject
{
    private readonly DatabaseService _dbService;
    private readonly INavigationService _navigation;
    private readonly IDialogService _dialogs;
    private List<ConversationRecord> _all = [];

    [ObservableProperty] private ObservableCollection<ConversationRecord> items = [];
    [ObservableProperty] private bool isRefreshing;
    [ObservableProperty] private string searchText = string.Empty;
    [ObservableProperty] private bool onlyVerified;

    public HistoryViewModel(DatabaseService dbService, INavigationService navigation, IDialogService dialogs)
    {
        _dbService = dbService;
        _navigation = navigation;
        _dialogs = dialogs;
    }

    partial void OnSearchTextChanged(string value) => ApplyFilter();
    partial void OnOnlyVerifiedChanged(bool value) => ApplyFilter();

    [RelayCommand]
    public async Task LoadItemsAsync()
    {
        IsRefreshing = true;
        try
        {
            _all = await _dbService.GetItemsAsync();

            // Muestra el nombre del proyecto en cada consulta.
            Dictionary<int, string> names = (await _dbService.GetProjectsAsync())
                .ToDictionary(p => p.Id, p => p.Name);
            foreach (ConversationRecord r in _all)
                r.ProjectName = names.GetValueOrDefault(r.ProjectId, string.Empty);

            ApplyFilter();
        }
        catch (Exception ex)
        {
            await _dialogs.AlertAsync("Error", $"No se pudo cargar el historial: {ex.Message}");
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    // RF06: filtra por texto (prompt, respuesta, categoría o proyecto) y por estado de verificación.
    private void ApplyFilter()
    {
        string term = SearchText?.Trim() ?? string.Empty;

        IEnumerable<ConversationRecord> query = _all;
        if (OnlyVerified)
            query = query.Where(r => r.IsVerified);
        if (term.Length > 0)
            query = query.Where(r =>
                r.Prompt.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                r.Answer.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                r.Category.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                r.ProjectName.Contains(term, StringComparison.OrdinalIgnoreCase));

        Items = new ObservableCollection<ConversationRecord>(query);
    }

    [RelayCommand]
    private Task OpenDetailAsync(ConversationRecord? record) =>
        record is null
            ? Task.CompletedTask
            : _navigation.GoToAsync(nameof(DetailPage),
                new Dictionary<string, object> { ["Record"] = record });

    [RelayCommand]
    private async Task DeleteAsync(ConversationRecord? record)
    {
        if (record is null) return;

        bool confirm = await _dialogs.ConfirmAsync(
            "Confirmar", "¿Desea eliminar esta consulta del historial?");
        if (!confirm) return;

        await _dbService.DeleteAsync(record);
        _all.Remove(record);
        Items.Remove(record);
    }
}
