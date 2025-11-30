using Cronus.DataAccess;
using Cronus.DataAccess.Model;
using Cronus.Utils;
using System.Linq.Expressions;

namespace Cronus.Runtime
{
    public class DbSet<T> where T : new()
    {
        private readonly Database _db;
        private readonly string _table;
        private readonly TableModel _tableModel;

        internal DbSet(Database db)
        {
            _db = db;
            _table = EntityMapper.GetTableName<T>();
            _tableModel = _db.Model.TablesSchema.First(t => t.Name == _table);

            if (!_db.Model.Data.ContainsKey(_table))
            {
                _db.Model.Data[_table] = [];
            }
        }

        private List<Dictionary<string, object?>> Rows => _db.Model.Data[_table];

        public IEnumerable<T> Select(Expression<Func<T, bool>>? predicate = null)
        {
            var query = Rows.Select(EntityMapper.FromRow<T>);

            if (predicate is not null)
            {
                query = query.Where(predicate.Compile());
            }

            return query;
        }

        public bool Insert(T entity)
        {
            var pkProperty = EntityMapper.GetPrimaryKey<T>();
            var pkHelper = new PropertyAttributeHelper(pkProperty);
            var pkColumnName = pkHelper.GetColumnName();
            var pkValue = pkProperty.GetValue(entity);

            bool IsPkEmpty =
                pkValue is null ||
                (pkValue is int i && i == 0) ||
                (pkValue is long l && l == 0L);

            if(IsPkEmpty)
            {
                int nextPk = 1;

                if (Rows.Any())
                {
                    nextPk = Rows
                        .Where(r => r.TryGetValue(pkColumnName, out var v) && v is not null)
                        .Select(r => Convert.ToInt32(r[pkColumnName]))
                        .DefaultIfEmpty(0)
                        .Max() + 1;
                }

                pkProperty.SetValue(entity, nextPk);
                pkValue = nextPk;
            }

            if (pkValue is not null)
            {
                var exists = Rows.Any(r => 
                r.TryGetValue(pkColumnName, out var existing) && KeyEqual(existing, pkValue));

                if (exists)
                {
                    return false;
                }
            }

            var row = EntityMapper.ToRow(entity);
            Rows.Add(row);
            return true;
        }

        public bool Update(T entity) 
        {
            var pkProperty = EntityMapper.GetPrimaryKey<T>();
            var pkHelper = new PropertyAttributeHelper(pkProperty);
            var pkColumnName = pkHelper.GetColumnName();
            var pkValue = pkProperty.GetValue(entity);

            var existingRecord = Rows.FirstOrDefault(r => r.TryGetValue(pkColumnName, out var value) && KeyEqual(value, pkValue));

            if (existingRecord is null) 
                return false;

            foreach (var property in typeof(T).GetProperties())
            {
                var helper = new PropertyAttributeHelper(property);

                if (helper.IsNotMapped())
                {
                    continue;
                }

                var columnName = helper.GetColumnName();

                if (columnName == pkColumnName)
                {
                    continue;
                }

                var newValue = property.GetValue(entity);

                if (Equals(newValue, GetDefault(property.PropertyType)))
                {
                    continue;
                }

                existingRecord[columnName] = newValue;
            }

            return true;  
        }

        private object? GetDefault(Type t)
        {
            return t.IsValueType ? Activator.CreateInstance(t) : null;
        }

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

        public int DeleteById(object id)
        {
            var pkProperty = EntityMapper.GetPrimaryKey<T>();
            var pkHelper = new PropertyAttributeHelper(pkProperty);
            var pkColumnName = pkHelper.GetColumnName();

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

        public IEnumerable<TParent> Include<TParent, TChild>(
            IEnumerable<TParent> parents, 
            Expression<Func<IEnumerable<TChild>>> navigation, 
            string mappedByFk)
            where TParent : class, new()
            where TChild : class, new()
        {
            var navProp = (navigation.Body as MemberExpression)?.Member
                ?? throw new ArgumentException("Navigation must be property");

            var parentPkName = EntityMapper.GetPrimaryKey<TParent>().Name;
            var childTable = EntityMapper.GetTableName<TChild>();
            var childRows = _db.Model.Data[childTable];

            foreach (var p in parents)
            {
                var parentId = p.GetType().GetProperty(parentPkName)!.GetValue(p);
                var children = childRows
                    .Where(r => r.TryGetValue(mappedByFk, out var value) && Equals(value, parentId))
                    .Select(EntityMapper.FromRow<TChild>).ToList();

                var propInfo = p.GetType().GetProperty(navProp.Name)!;
                propInfo.SetValue(p, children);
            }

            return parents;
        }

        public IEnumerable<TLeft> IncludeManyToMany<TLeft, TRight>(
            IEnumerable<TLeft> lefts, 
            Expression<Func<TLeft, IEnumerable<TRight>>> navigation)
            where TLeft : class, new()
            where TRight : class, new()
        {
            var navProp = (navigation.Body as MemberExpression)?.Member
                ?? throw new ArgumentException("Navigation must be property");

            var leftTable = EntityMapper.GetTableName<TLeft>();
            var rightTable = EntityMapper.GetTableName<TRight>();

            var joinTable = string.CompareOrdinal(leftTable, rightTable) < 0
                ? $"{leftTable}_{rightTable}" : $"{rightTable}_{leftTable}";
            var leftPk = EntityMapper.GetPrimaryKey<TLeft>().Name;
            var rightPk = EntityMapper.GetPrimaryKey<TRight>().Name;
            var leftKeyCol = $"{leftTable.Trim('s')}Id";
            var rightKeyCol = $"{rightTable.Trim('s')}Id";

            if (!_db.Model.Data.TryGetValue(joinTable, out var joinRows))
                return lefts;

            var rightRows = _db.Model.Data[rightTable];

            foreach (var l in lefts)
            {
                var id = l.GetType().GetProperty(leftPk)!.GetValue(l);
                var rightIds = joinRows
                    .Where(r => Equals(r[leftKeyCol], id))
                    .Select(r => r[rightKeyCol])
                    .ToHashSet();

                var rights = rightRows
                    .Where(r => rightIds.Contains(r[rightPk]))
                    .Select(EntityMapper.FromRow<TRight>)
                    .ToList();

                var propInfo = l.GetType().GetProperty(navProp.Name)!;
                propInfo.SetValue(l, rights);
            }

            return lefts;
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

        private static bool KeyEqual(object? a, object? b)
        {
            if (a is null && b is null) return true;
            if (a is null || b is null) return false;

            if (a is IConvertible && b is IConvertible)
            {
                try
                {
                    var da = Convert.ToInt64(a);
                    var db = Convert.ToInt64(b);
                    return da == db;
                }
                catch
                {

                }
            }

            return a.Equals(b);
        }
    }
}
