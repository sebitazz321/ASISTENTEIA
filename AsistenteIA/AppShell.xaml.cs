using AsistenteIA.Views;

namespace AsistenteIA;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Registro de ruta para navegación secundaria
        Routing.RegisterRoute(nameof(DetailPage), typeof(DetailPage));
    }
}