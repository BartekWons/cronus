using Cronus.Utils;
using System.Reflection;

namespace Cronus.Runtime
{
    internal static class EntityMapper
    {
        internal static string GetTableName<T>()
        {
            return new TypeAttributeHelper(typeof(T)).GetTableName()!;
        }

        internal static PropertyInfo GetPrimaryKey<T>()
        {
            var primaryKeyInfo = new TypeAttributeHelper(typeof(T)).GetPrimaryKeyInfo() 
                ?? throw new InvalidOperationException($"{typeof(T).Name} has no Primary Key");
            return primaryKeyInfo;
        }

        internal static object? GetPrimaryKeyValue<T>(T entity)
        {
            return GetPrimaryKey<T>().GetValue(entity);
        }

        internal static Dictionary<string, object?> ToRow<T>(T entity)
        {
            var dict = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
            foreach (var property in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var helper = new PropertyAttributeHelper(property);

                if (helper.IsNotMapped())
                {
                    continue;
                }

                var col = helper.GetColumnName();
                dict[col] = property.GetValue(entity);
            }

            return dict;
        }

        internal static T FromRow<T>(IDictionary<string, object?> row) where T : new()
        {
            var obj = new T();

            foreach (var property in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var helper = new PropertyAttributeHelper(property);

                if (helper.IsNotMapped())
                {
                    continue;
                }

                var col = helper.GetColumnName(); 

                if (row.TryGetValue(col, out var val))
                {
                    if (val is null)
                    {
                        property.SetValue(obj, null);
                        continue;
                    }

                    var target = property.PropertyType;

                    property.SetValue(obj, Convert.ChangeType(val, Nullable.GetUnderlyingType(target) ?? target), null);
                }
            }

            return obj;
        }
    }
}
