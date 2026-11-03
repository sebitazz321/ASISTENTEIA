using AsistenteIA.ViewModels;

namespace AsistenteIA.Views;

public partial class ProjectDetailPage : ContentPage
{
    public ProjectDetailPage(ProjectDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
