using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;
using Noted;

namespace Noted
{
    public static class NotedDatabase
    {
        private static SQLiteAsyncConnection Database { get; set; }

        /// <summary>
        /// Initializes the connection to the database. If the connection has already been initialized, it will not be initialized again. This method only needs to be called once during the lifetime of the application.
        /// </summary>
        /// <param name="dbPath"></param>
        public static void InitializeConnection(string dbPath)
        {
            if (Database == null)
            {
                SQLiteOpenFlags sQLiteOpenFlags = SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.SharedCache;
                Database = new SQLiteAsyncConnection(dbPath, sQLiteOpenFlags, true);
                Database.CreateTableAsync<NotedModel>().Wait();
            }
        }

        public static async Task<List<NotedModel>> GetNotesAsync()
        {
            // Get all notes.
            return await Database.Table<NotedModel>().ToListAsync();
        }

        public static async Task<NotedModel> GetNoteAsync(string note)
        {
            // Get a specific note.
            return await Database.Table<NotedModel>()
                            .Where(noted => noted.NoteName == note)
                            .FirstOrDefaultAsync();
        }

        public static async Task<int> SaveNoteAsync(NotedModel note)
        {
            // Update an existing note.
            return await Database.InsertAsync(note);
        }

        public static async Task<int> DeleteNoteAsync(NotedModel note)
        {
            // Delete a note.
            return await Database.DeleteAsync(note);
        }

        public static async Task<int> UpdateNoteAsync(NotedModel note)
        {
            // Update a note.
            return await Database.UpdateAsync(note);
        }
    }
}