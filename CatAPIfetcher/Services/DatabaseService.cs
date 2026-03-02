using SQLite;
using CatAPIfetcher.Model;

namespace CatAPIfetcher.Services
{
    public class DatabaseService
    {
        private SQLiteAsyncConnection _database;

        public DatabaseService()
        {
        }

        private async Task InitAsync()
        {
            if (_database != null)
                return;

            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "cats.db3");
            _database = new SQLiteAsyncConnection(dbPath);
            await _database.CreateTableAsync<Cat>();
        }

        public async Task<List<Cat>> GetCatsAsync()
        {
            await InitAsync();
            return await _database.Table<Cat>().OrderByDescending(c => c.CreatedAt).ToListAsync();
        }

        public async Task<Cat> GetCatAsync(string id)
        {
            await InitAsync();
            return await _database.Table<Cat>().Where(c => c.Id == id).FirstOrDefaultAsync();
        }

        public async Task<int> SaveCatAsync(Cat cat)
        {
            await InitAsync();

            var existing = await GetCatAsync(cat.Id);
            if (existing != null)
            {
                return await _database.UpdateAsync(cat);
            }
            else
            {
                return await _database.InsertAsync(cat);
            }
        }

        public async Task<int> SaveCatsAsync(List<Cat> cats)
        {
            await InitAsync();
            var count = 0;
            foreach (var cat in cats)
            {
                count += await SaveCatAsync(cat);
            }
            return count;
        }

        public async Task<int> DeleteCatAsync(Cat cat)
        {
            await InitAsync();
            return await _database.DeleteAsync(cat);
        }

        public async Task<int> ClearAllCatsAsync()
        {
            await InitAsync();
            return await _database.DeleteAllAsync<Cat>();
        }
    }
}