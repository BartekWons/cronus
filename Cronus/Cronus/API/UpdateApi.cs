using Cronus.DataAccess;
using Cronus.Utils;

namespace Cronus.API
{
    internal class UpdateApi<T> : ApiBase<T> where T : new()
    {
        public UpdateApi(Database db) : base(db) { }

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
    }
}
