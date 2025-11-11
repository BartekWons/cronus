using Cronus.DataAccess;
using Cronus.Utils;

namespace Cronus.Runtime
{
    public class Db
    {
        private readonly Database _db;
        private readonly DatabaseModelFileHandler _fileHandler;

        internal Db(Database db)
        {
            _db = db;
            _fileHandler = new DatabaseModelFileHandler(_db.Model);
        }

        public DbSet<T> Set<T>() where T : class, new()
        {
            return new DbSet<T>(_db);
        }

        public async Task SaveChangesAsync()
        {
            await _fileHandler.SaveAsync(_db.Config.ConnectionString);
        }
    }
}
