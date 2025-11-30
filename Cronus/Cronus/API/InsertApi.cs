using Cronus.DataAccess;
using Cronus.Utils;

namespace Cronus.API
{
    internal class InsertApi<T> : ApiBase<T> where T : new()
    {
        public InsertApi(Database db) : base(db) { }

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

            if (IsPkEmpty)
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
    }
}
