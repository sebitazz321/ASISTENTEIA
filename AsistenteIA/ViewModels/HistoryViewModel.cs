using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AsistenteIA.Models;
using AsistenteIA.Services;

namespace AsistenteIA.ViewModels;

public partial class HistoryViewModel : ObservableObject
{
    private readonly DatabaseService _dbService;

    [ObservableProperty]
    private ObservableCollection<ConversationRecord> items = [];

    [ObservableProperty]
    private bool isRefreshing;

    public HistoryViewModel(DatabaseService dbService)
    {
        _dbService = dbService;
    }

    [RelayCommand]
    public async Task LoadItemsAsync()
    {
        IsRefreshing = true;
        try
        {
            var list = await _dbService.GetItemsAsync();
            Items = new ObservableCollection<ConversationRecord>(list);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"No se pudo cargar el historial: {ex.Message}", "OK");
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    private async Task DeleteAsync(ConversationRecord record)
    {
        if (record is null) return;

        bool confirm = await Shell.Current.DisplayAlert(
            "Confirmar", "¿Desea eliminar esta consulta del historial?", "Sí", "No");

        if (confirm)
        {
            await _dbService.DeleteAsync(record);
            Items.Remove(record);
        }
    }
}