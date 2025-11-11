using Cronus.DataAccess;
using Cronus.DataAccess.Model;
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
            return false;
        }

        public bool Update(T entity) 
        {
            return false;  
        }

        public int DeleteById(object id)
        {
            return default;
        }
    }
}
