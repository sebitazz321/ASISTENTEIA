using AsistenteIA.Models;

namespace AsistenteIA.Services;

/// <summary>Perfil del estudiante y preferencias de uso. Todo se guarda solo en el dispositivo.</summary>
public class ProfileService
{
    private const string NameKey = "profile_name";
    private const string ProgramKey = "profile_program";
    private const string InstitutionKey = "profile_institution";
    private const string NoticeKey = "responsible_use_notice_accepted";

    public UserProfile Load() => new()
    {
        Name = Preferences.Get(NameKey, string.Empty),
        Program = Preferences.Get(ProgramKey, string.Empty),
        Institution = Preferences.Get(InstitutionKey, string.Empty)
    };

    public void Save(UserProfile profile)
    {
        Preferences.Set(NameKey, profile.Name.Trim());
        Preferences.Set(ProgramKey, profile.Program.Trim());
        Preferences.Set(InstitutionKey, profile.Institution.Trim());
    }

    public void Clear()
    {
        Preferences.Remove(NameKey);
        Preferences.Remove(ProgramKey);
        Preferences.Remove(InstitutionKey);
    }

    /// <summary>Aviso de uso responsable de la IA: se muestra una vez al abrir la app.</summary>
    public bool NoticeAccepted
    {
        get => Preferences.Get(NoticeKey, false);
        set => Preferences.Set(NoticeKey, value);
    }
}
