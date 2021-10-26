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
                Database = new SQLiteAsyncConnection(dbPath);
                Database.CreateTableAsync<NotedModel>().Wait();
            }
        }

        public static Task<List<NotedModel>> GetNotesAsync()
        {
            // Get all notes.
            return Database.Table<NotedModel>().ToListAsync();
        }

        public static Task<NotedModel> GetNoteAsync(string note)
        {
            // Get a specific note.
            return Database.Table<NotedModel>()
                            .Where(noted => noted.NoteName == note)
                            .FirstOrDefaultAsync();
        }

        public static Task<int> SaveNoteAsync(NotedModel note)
        {
            // Update an existing note.
            return Database.InsertAsync(note);
        }

        public static Task<int> DeleteNoteAsync(NotedModel note)
        {
            // Delete a note.
            return Database.DeleteAsync(note);
        }

        public static Task<int> UpdateNoteAsync(NotedModel note)
        {
            // Update a note.
            return Database.UpdateAsync(note);
        }
    }
}