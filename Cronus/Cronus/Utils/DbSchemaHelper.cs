using Cronus.DataAccess;
using Cronus.DataAccess.Model;
using System.Net.Http.Headers;

namespace Cronus.Utils
{
    internal class DbSchemaHelper
    {
        private readonly Database _db;

        public DbSchemaHelper(Database db)
        {
            _db = db;
        }

        public TableModel GetTableModel(string tableName)
        {
            var model = _db.Model.TablesSchema.FirstOrDefault(t => t.Name.Equals(tableName, StringComparison.OrdinalIgnoreCase));
            return model ?? throw new InvalidOperationException($"Table: {tableName} does not exist in the schema.");
        }

        public string GetPrimaryKeyName(string tableName)
        {
            var model = GetTableModel(tableName);
            var primaryKey = model.Columns?.FirstOrDefault(c => c.IsPrimaryKey)
                ?? throw new InvalidOperationException($"Table: {tableName} has no primary key.");
            return primaryKey.Name;
        }
    }
}
