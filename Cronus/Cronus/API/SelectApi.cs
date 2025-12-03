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

        public IEnumerable<TParent> Join<TParent, TChild>(
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
                    .Where(r => r.TryGetValue(mappedByFk, out var value) && KeyEqual(value, parentId))
                    .Select(EntityMapper.FromRow<TChild>).ToList();

                var propInfo = p.GetType().GetProperty(navProp.Name)!;
                propInfo.SetValue(p, children);
            }

            return parents;
        }
    }
}
