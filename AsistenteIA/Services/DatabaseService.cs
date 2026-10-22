using SQLite;
using AsistenteIA.Models;

namespace AsistenteIA.Services;

public class DatabaseService
{
    private SQLiteAsyncConnection? _database;

    private async Task InitAsync()
    {
        if (_database is not null) return;

        try
        {
            SQLitePCL.Batteries_V2.Init();
        }
        catch { }

        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "AsistenteIA.db3");
        _database = new SQLiteAsyncConnection(dbPath);
        await _database.CreateTableAsync<ConversationRecord>();
    }

    public async Task<List<ConversationRecord>> GetItemsAsync()
    {
        await InitAsync();
        return await _database!.Table<ConversationRecord>()
                                .OrderByDescending(x => x.CreatedAt)
                                .ToListAsync();
    }

    public async Task<int> SaveAsync(ConversationRecord item)
    {
        await InitAsync();
        if (item.Id != 0)
            return await _database!.UpdateAsync(item);

        return await _database!.InsertAsync(item);
    }

    public async Task<int> DeleteAsync(ConversationRecord item)
    {
        await InitAsync();
        return await _database!.DeleteAsync(item);
    }
}