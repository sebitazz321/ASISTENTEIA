using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AsistenteIA.Models;
using AsistenteIA.Services;

namespace AsistenteIA.ViewModels;

// Permite abrir la consulta con un proyecto preseleccionado: //ChatPage?projectId=3
[QueryProperty(nameof(StartProjectId), "projectId")]
public partial class ChatViewModel : ObservableObject
{
    private readonly AiService _aiService;
    private readonly DatabaseService _dbService;
    private readonly IDialogService _dialogs;
    private CancellationTokenSource? _cts;
    private int? _pendingProjectId;

    // Datos de la última respuesta (el usuario puede editar el prompt después de consultar).
    private string _lastPrompt = string.Empty;
    private string _lastCategory = "General";
    private int _lastProjectId;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CharCountText))]
    private string prompt = string.Empty;

    [ObservableProperty] private string answer = string.Empty;
    [ObservableProperty] private string statusMessage = string.Empty;
    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private PromptTemplate? selectedTemplate;
    [ObservableProperty] private Project? selectedProject = Project.None;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private bool hasUnsavedAnswer;

    public ObservableCollection<PromptTemplate> Templates { get; } =
        new(PromptTemplate.Defaults);

    public ObservableCollection<Project> Projects { get; } = [Project.None];

    public int MaxLength => AiService.MaxPromptLength;
    public string CharCountText => $"{Prompt?.Length ?? 0} / {AiService.MaxPromptLength}";

    /// <summary>Recibe el id del proyecto desde la navegación (Shell query).</summary>
    public string? StartProjectId
    {
        get => null;
        set
        {
            if (int.TryParse(value, out int id))
            {
                _pendingProjectId = id;
                ApplyPendingProject();
            }
        }
    }

    public ChatViewModel(AiService aiService, DatabaseService dbService, IDialogService dialogs)
    {
        _aiService = aiService;
        _dbService = dbService;
        _dialogs = dialogs;
        SelectedTemplate = Templates[0];
    }

    // RF07: al elegir una plantilla se carga en el editor para poder modificarla.
    partial void OnSelectedTemplateChanged(PromptTemplate? value)
    {
        if (value is not null)
            Prompt = value.Text;
    }

    /// <summary>Recarga la lista de proyectos (se llama cada vez que aparece la pantalla).</summary>
    public async Task LoadProjectsAsync()
    {
        try
        {
            List<Project> list = await _dbService.GetProjectsAsync();
            int currentId = SelectedProject?.Id ?? 0;

            Projects.Clear();
            Projects.Add(Project.None);
            foreach (Project p in list)
                Projects.Add(p);

            SelectedProject = Projects.FirstOrDefault(p => p.Id == currentId) ?? Project.None;
            ApplyPendingProject();
            _pendingProjectId = null;
        }
        catch (Exception ex)
        {
            StatusMessage = $"No se pudieron cargar los proyectos: {ex.Message}";
        }
    }

    private void ApplyPendingProject()
    {
        if (_pendingProjectId is not int id) return;

        Project? match = Projects.FirstOrDefault(p => p.Id == id);
        if (match is not null)
        {
            SelectedProject = match;
            _pendingProjectId = null;
        }
    }

    [RelayCommand]
    private async Task AskAsync()
    {
        if (IsBusy) return;

        // RF02: validación antes de enviar
        if (PromptValidator.Validate(Prompt) is { } validationError)
        {
            StatusMessage = validationError;
            return;
        }
        string clean = Prompt.Trim();

        _cts?.Dispose();
        _cts = new CancellationTokenSource();
        bool cancelledByUser = false;
        IsBusy = true;

        try
        {
            // Privacidad: avisar antes de enviar datos personales a un proveedor externo.
            IReadOnlyList<string> sensitive = SensitiveDataDetector.Detect(clean);
            if (sensitive.Count > 0)
            {
                bool send = await _dialogs.ConfirmAsync(
                    "Posibles datos sensibles",
                    $"La consulta parece incluir: {string.Join(", ", sensitive)}.\n\n" +
                    "El texto se envía a un proveedor de IA externo. ¿Desea enviarla de todas formas?",
                    "Enviar", "Revisar");
                if (!send)
                {
                    StatusMessage = "Envío cancelado: quite los datos personales y vuelva a intentar.";
                    return;
                }
            }

            HasUnsavedAnswer = false;
            Answer = string.Empty;
            StatusMessage = $"Consultando {AiService.CurrentProviderInfo.DisplayName}...";

            Project? project = SelectedProject is { Id: > 0 } p ? p : null;
            string? context = project is null ? null : $"{project.Name}. {project.Description}";

            Answer = await _aiService.AskAsync(clean, null, _cts.Token, context);
            _lastPrompt = clean;
            _lastCategory = SelectedTemplate?.Category ?? "General";
            _lastProjectId = project?.Id ?? 0;
            HasUnsavedAnswer = true;
            StatusMessage = $"Respuesta recibida de {AiService.CurrentProviderInfo.DisplayName}. " +
                            "La IA puede equivocarse: verifique la información. " +
                            "Pulse «Guardar» para añadirla al historial.";
        }
        catch (Exception ex)
        {
            cancelledByUser = _cts.IsCancellationRequested;
            StatusMessage = AiService.GetFriendlyMessage(ex, cancelledByUser);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void Cancel() => _cts?.Cancel();

    private bool CanSave() => HasUnsavedAnswer;

    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task SaveAsync()
    {
        try
        {
            await _dbService.SaveAsync(new ConversationRecord
            {
                Prompt = _lastPrompt,
                Answer = Answer,
                CreatedAt = DateTime.UtcNow,
                Category = _lastCategory,
                ProjectId = _lastProjectId,
                IsVerified = false
            });
            HasUnsavedAnswer = false;
            StatusMessage = "Consulta guardada en el historial.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"No se pudo guardar localmente: {ex.Message}";
        }
    }
}
