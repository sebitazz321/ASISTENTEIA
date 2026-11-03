using Microsoft.Extensions.DependencyInjection;

namespace AsistenteIA
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            UserAppTheme = AppTheme.Dark; // estilo oscuro moderno en todos los dispositivos
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}