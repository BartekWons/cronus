using Cronus.DataAccess;
using Cronus.DataAccess.Model;
using Cronus.Interfaces;
using Cronus.Parser;
using Cronus.Utils;
using System.Data;

namespace Cronus.Runtime
{
    internal class DatabaseAdapter : IDatabaseAdapter
    {
        private readonly Database _db;
        private readonly DbSchemaHelper _schemaHelper;
        public DatabaseAdapter(Database db)
        {
            _db = db;
            _schemaHelper = new DbSchemaHelper(db);
        }

        public Task<int> DeleteAsync(string table, ICondition? where)
        {
            if (!_db.Model.Data.TryGetValue(table, out var rows))
            {
                return Task.FromResult(0);
            }

            var toDelete = rows.Where(r => Matches(where, r)).ToList();
            var totalRemoved = 0;

            foreach(var row in toDelete)
            {
                totalRemoved += DeleteWithCascade(table, row);
            }

            return Task.FromResult(totalRemoved);
        }

        public Task InsertAsync(string table, IReadOnlyDictionary<string, object>? values)
        {
            if (values is null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            var tableSchema = _schemaHelper.GetTableModel(table);

            if (!_db.Model.Data.TryGetValue(table, out var rows))
            {
                rows = [];
                _db.Model.Data[table] = rows;
            }

            var row = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

            foreach (var col in tableSchema.Columns ?? Enumerable.Empty<ColumnModel>())
            {
                if (!values.TryGetValue(col.Name, out var v))
                {
                    row[col.Name] = null;
                    continue;
                }

                row[col.Name] = v;
            }

            rows.Add(row);

            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<IDictionary<string, object?>>> SelectAsync(string table, IReadOnlyList<string> columns, ICondition? whereCondition)
        {
            if (!_db.Model.Data.TryGetValue(table, out var rows))
                throw new InvalidOperationException($"Table: {table} does not exist.");

            var filtered = rows.Where(r => Matches(whereCondition, r)).ToList();

            var result = new List<IDictionary<string, object?>>();

            bool selectAll = columns.Count == 1 && columns[0] == "*";

            foreach (var row in filtered)
            {
                if (selectAll)
                {
                    result.Add(new Dictionary<string, object?>(row, StringComparer.OrdinalIgnoreCase));
                    continue;
                }

                var dict = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

                foreach (var col in columns)
                {
                    row.TryGetValue(col, out var value);
                    dict[col] = value;
                }

                result.Add(dict);
            }

            return Task.FromResult((IReadOnlyList<IDictionary<string, object?>>)result);
        }

        public Task<int> UpdateAsync(string table, IReadOnlyDictionary<string, object>? values, ICondition? whereCondition)
        {
            if (values is null || values.Count == 0)
            {
                return Task.FromResult(0);
            }

            if(!_db.Model.Data.TryGetValue(table, out var rows))
            {
                return Task.FromResult(0);
            }

            var affected = 0;

            foreach (var row in rows)
            {
                if (!Matches(whereCondition, row))
                {
                    continue;
                }

                foreach (var kvp in values)
                {
                    row[kvp.Key] = kvp.Value;
                }

                affected++;
            }

            return Task.FromResult(affected);
        }

        private bool Matches(ICondition? where, IDictionary<string, object>? row)
        {
            if (where is null)
            {
                return true;
            }

            if (where is BinaryCondition bc)
            {
                if (!row.TryGetValue(bc.Left, out var value))
                {
                    return false;
                }

                return Equals(value, bc.Right);
            }

            throw new NotSupportedException("Only simple binary conditions are supported");
        }

        private int DeleteWithCascade(string table, IDictionary<string, object?> row)
        {
            var removed = 0;

            var pkName = _schemaHelper.GetPrimaryKeyName(table);
            var pkValue = row[pkName];

            foreach (var childTable in _db.Model.TablesSchema)
            {
                foreach (var fk in childTable.ForeignKeys
                    .Where(fk => fk.ReferencedTable.Equals(
                        table, StringComparison.OrdinalIgnoreCase) && fk.CascadeDelete))
                {
                    if (_db.Model.Data.TryGetValue(childTable.Name, out var childRows))
                    {
                        continue;
                    }

                    var toRemovedChildren = childRows
                        .Where(r => r.TryGetValue(fk.Column, out var val) && Equals(val, pkValue))
                        .ToList();

                    foreach (var childRow in toRemovedChildren)
                    {
                        removed += DeleteWithCascade(childTable.Name, childRow);
                    }
                }
            }

            if (_db.Model.Data.TryGetValue(table, out var rows))
            {
                if (rows.Remove(row.ToDictionary()))
                    removed++;
            }

            return removed;
        }
    }
}
