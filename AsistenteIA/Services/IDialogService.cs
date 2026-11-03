namespace AsistenteIA.Services;

/// <summary>Abstrae los cuadros de diálogo para que los ViewModels no dependan de la interfaz.</summary>
public interface IDialogService
{
    Task AlertAsync(string title, string message, string cancel = "Aceptar");
    Task<bool> ConfirmAsync(string title, string message, string accept = "Sí", string cancel = "No");
}

public class ShellDialogService : IDialogService
{
    public Task AlertAsync(string title, string message, string cancel = "Aceptar") =>
        Shell.Current.DisplayAlert(title, message, cancel);

    public Task<bool> ConfirmAsync(string title, string message, string accept = "Sí", string cancel = "No") =>
        Shell.Current.DisplayAlert(title, message, accept, cancel);
}
