using Cronus.DataAccess;
using Cronus.Interfaces;
using Cronus.Parser.QueryExecutors;
using Cronus.Utils;

namespace Cronus.Runtime
{
    public class Db
    {
        private readonly Database _db;
        private readonly DatabaseModelLoader _fileHandler;
        private readonly ISqlQueryExecutor _sqlExecutor;

        internal Db(Database db)
        {
            _db = db;
            _fileHandler = new DatabaseModelLoader(_db.Model);

            var adapter = new DatabaseAdapter(_db);
            _sqlExecutor = new SqlQueryExecutor(adapter);
        }

        public Task<object?> Query(string sql)
        {
            return _sqlExecutor.ExecuteAsync(sql);
        }

        public DbSet<T> GetTable<T>() where T : class, new()
        {
            return new DbSet<T>(_db);
        }

        public async Task SaveChangesAsync()
        {
            await _fileHandler.SaveAsync(_db.Config.ConnectionString);
        }
    }
}
