using Cronus.DataAccess;
using Cronus.Utils;

namespace Cronus.API
{
    internal class ApiBase<T> where T : new()
    {
        protected readonly Database _db;
        private readonly string _table;

        protected ApiBase(Database db)
        {
            _db = db;
            _table = EntityMapper.GetTableName<T>();

            if (!_db.Model.Data.ContainsKey(_table))
            {
                _db.Model.Data[_table] = [];
            }
        }

        protected List<Dictionary<string, object?>> Rows => _db.Model.Data[_table];

        protected bool KeyEqual(object? a, object? b)
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
