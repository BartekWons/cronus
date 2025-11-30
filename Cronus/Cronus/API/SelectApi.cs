using Cronus.DataAccess;
using Cronus.Utils;
using System.Linq.Expressions;

namespace Cronus.API
{
    internal class SelectApi<T> : ApiBase<T> where T : new()
    {
        public SelectApi(Database db) : base(db) { }

        public IEnumerable<T> Select(Expression<Func<T, bool>>? predicate = null)
        {
            var query = Rows.Select(EntityMapper.FromRow<T>);

            if (predicate is not null)
            {
                query = query.Where(predicate.Compile());
            }

            return query;
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
    }
}
