using Cronus.DataAccess;
using Cronus.Utils;
using System.Linq.Expressions;

namespace Cronus.API
{
    internal class DeleteApi<T> : ApiBase<T> where T : new()
    {
        public DeleteApi(Database db) : base(db) { }

        public int Delete(Expression<Func<T, bool>> predicate)
        {
            var compiled = predicate.Compile();

            var pkProperty = EntityMapper.GetPrimaryKey<T>();

            var idsToDelete = Rows
                .Select(EntityMapper.FromRow<T>)
                .Where(compiled)
                .Select(e => pkProperty.GetValue(e))
                .ToList();

            var total = 0;

            foreach (var id in idsToDelete)
            {
                total += DeleteById(id);
            }

            return total;
        }

        private int DeleteById(object id)
        {
            var pkProperty = EntityMapper.GetPrimaryKey<T>();
            var pkHelper = new PropertyAttributeHelper(pkProperty);
            var pkColumnName = pkHelper.GetColumnName();
            var _table = EntityMapper.GetTableName<T>();

            var toRemove = Rows.Where(r => r.TryGetValue(pkColumnName, out var value) && KeyEqual(value, id)).ToList();

            if (toRemove.Count == 0)
            {
                return 0;
            }

            var totalRemoved = 0;

            foreach (var row in toRemove)
            {
                var pkValue = row[pkColumnName];

                foreach (var child in _db.Model.TablesSchema)
                {
                    foreach (var fk in child.ForeignKeys.Where(fk => fk.ReferencedTable == _table && fk.ReferencedColumn == pkColumnName))
                    {
                        totalRemoved += DeleteChildren(child.Name, fk.Column, pkValue);
                    }
                }

                Rows.Remove(row);
                totalRemoved++;
            }

            return totalRemoved;
        }

        private int DeleteChildren(string parentTable, string fkColumn, object parentId)
        {
            if (!_db.Model.Data.TryGetValue(parentTable, out var list))
                return 0;

            var schema = _db.Model.TablesSchema.First(t => t.Name == parentTable);
            var pkName = schema.Columns!.First(c => c.IsPrimaryKey).Name;

            var toRemove = list
                .Where(r => r.TryGetValue(fkColumn, out var value) && Equals(value, parentId))
                .ToList();

            var removed = 0;

            foreach (var row in toRemove)
            {
                list.Remove(row);
                removed++;

                var childId = row[pkName];

                foreach (var grand in _db.Model.TablesSchema)
                {
                    foreach (var grandFk in grand.ForeignKeys
                        .Where(fk => fk.ReferencedTable == parentTable && fk.ReferencedColumn == pkName))
                    {
                        removed += DeleteChildren(grand.Name, grandFk.Column, childId!);
                    }
                }
            }

            return removed;
        }
    }
}
