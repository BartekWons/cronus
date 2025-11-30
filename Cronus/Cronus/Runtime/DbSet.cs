using Cronus.API;
using Cronus.DataAccess;
using System.Linq.Expressions;

namespace Cronus.Runtime
{
    public class DbSet<T> where T : new()
    {
        private readonly SelectApi<T> _selectApi;
        private readonly InsertApi<T> _insertApi;
        private readonly UpdateApi<T> _updateApi; 
        private readonly DeleteApi<T> _deleteApi; 

        public DbSet(Database db)
        {
            _selectApi = new SelectApi<T>(db);
            _insertApi = new InsertApi<T>(db);
            _updateApi = new UpdateApi<T>(db);
            _deleteApi = new DeleteApi<T>(db);
        }

        public IEnumerable<T> Select(Expression<Func<T, bool>>? predicate = null)
        {
            return _selectApi.Select(predicate);
        }

        public bool Insert(T entity)
        {
            return _insertApi.Insert(entity);
        }

        public bool Update(T entity)
        {
            return _updateApi.Update(entity);
        }

        public int Delete(Expression<Func<T, bool>> predicate)
        {
            return _deleteApi.Delete(predicate);
        }

        public IEnumerable<TParent> Include<TParent, TChild>(
            IEnumerable<TParent> parents,
            Expression<Func<IEnumerable<TChild>>> navigation,
            string mappedByFk)
            where TParent : class, new()
            where TChild : class, new()
        {
            return _selectApi.Include(parents, navigation, mappedByFk);
        }

        public IEnumerable<TLeft> IncludeManyToMany<TLeft, TRight>(
            IEnumerable<TLeft> lefts,
            Expression<Func<TLeft, IEnumerable<TRight>>> navigation)
            where TLeft : class, new()
            where TRight : class, new()
        {
            return _selectApi.IncludeManyToMany(lefts, navigation);
        }
    }
}
