using SQLite;
using AsistenteIA.Models;

namespace AsistenteIA.Services;

/// <summary>CRUD local en SQLite: historial de consultas y proyectos.</summary>
public class DatabaseService
{
    private SQLiteAsyncConnection? _database;

    private async Task InitAsync()
    {
        if (_database is not null) return;

        SQLitePCL.Batteries_V2.Init();

        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "AsistenteIA.db3");
        var connection = new SQLiteAsyncConnection(dbPath);
        // CreateTable también agrega columnas nuevas (ProjectId, Note) a bases ya existentes.
        await connection.CreateTableAsync<ConversationRecord>();
        await connection.CreateTableAsync<Project>();
        _database = connection;
    }

    // ---------------- Historial de consultas ----------------

    public async Task<List<ConversationRecord>> GetItemsAsync()
    {
        await InitAsync();
        return await _database!.Table<ConversationRecord>()
                               .OrderByDescending(x => x.CreatedAt)
                               .ToListAsync();
    }

    public async Task<List<ConversationRecord>> GetItemsByProjectAsync(int projectId)
    {
        await InitAsync();
        return await _database!.Table<ConversationRecord>()
                               .Where(x => x.ProjectId == projectId)
                               .OrderByDescending(x => x.CreatedAt)
                               .ToListAsync();
    }

    /// <summary>Inserta si Id == 0; de lo contrario actualiza.</summary>
    public async Task<int> SaveAsync(ConversationRecord item)
    {
        await InitAsync();
        return item.Id != 0
            ? await _database!.UpdateAsync(item)
            : await _database!.InsertAsync(item);
    }

    public async Task<int> DeleteAsync(ConversationRecord item)
    {
        await InitAsync();
        return await _database!.DeleteAsync(item);
    }

    public async Task<int> DeleteAllItemsAsync()
    {
        await InitAsync();
        return await _database!.DeleteAllAsync<ConversationRecord>();
    }

    // ---------------- Proyectos ----------------

    public async Task<List<Project>> GetProjectsAsync()
    {
        await InitAsync();
        SQLiteAsyncConnection db = _database!;
        List<Project> projects = await db.Table<Project>()
                                         .OrderBy(p => p.Name)
                                         .ToListAsync();
        foreach (Project p in projects)
        {
            p.ConversationCount = await db.Table<ConversationRecord>()
                                                 .Where(c => c.ProjectId == p.Id)
                                                 .CountAsync();
        }
        return projects;
    }

    /// <summary>Inserta si Id == 0; de lo contrario actualiza.</summary>
    public async Task<int> SaveProjectAsync(Project project)
    {
        await InitAsync();
        return project.Id != 0
            ? await _database!.UpdateAsync(project)
            : await _database!.InsertAsync(project);
    }

    /// <summary>Elimina el proyecto; sus consultas se conservan en el historial sin proyecto.</summary>
    public async Task DeleteProjectAsync(Project project)
    {
        await InitAsync();
        SQLiteAsyncConnection db = _database!;
        await db.ExecuteAsync(
            "UPDATE ConversationRecord SET ProjectId = 0 WHERE ProjectId = ?", project.Id);
        await db.DeleteAsync(project);
    }

    public async Task<int> DeleteAllProjectsAsync()
    {
        await InitAsync();
        return await _database!.DeleteAllAsync<Project>();
    }

    // ---------------- Estadísticas del perfil ----------------

    public async Task<(int Total, int Verified, int Projects)> GetStatsAsync()
    {
        await InitAsync();
        SQLiteAsyncConnection db = _database!;
        int total = await db.Table<ConversationRecord>().CountAsync();
        int verified = await db.Table<ConversationRecord>().Where(c => c.IsVerified).CountAsync();
        int projects = await db.Table<Project>().CountAsync();
        return (total, verified, projects);
    }
}
