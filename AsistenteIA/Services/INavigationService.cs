namespace AsistenteIA.Services;

/// <summary>Abstrae la navegación para que los ViewModels no dependan de Shell directamente (MVVM).</summary>
public interface INavigationService
{
    Task GoToAsync(string route, IDictionary<string, object>? parameters = null);
    Task GoBackAsync();
}

/// <summary>Implementación basada en .NET MAUI Shell.</summary>
public class ShellNavigationService : INavigationService
{
    public Task GoToAsync(string route, IDictionary<string, object>? parameters = null) =>
        parameters is null
            ? Shell.Current.GoToAsync(route)
            : Shell.Current.GoToAsync(route, parameters);

    public Task GoBackAsync() => Shell.Current.GoToAsync("..");
}
