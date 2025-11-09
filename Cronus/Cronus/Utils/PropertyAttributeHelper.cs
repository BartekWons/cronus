using Cronus.Attributes;
using System.Reflection;
using CronusAttributes = Cronus.Attributes;

namespace Cronus.Utils;

public class PropertyAttributeHelper
{
    private readonly PropertyInfo _property;

    public PropertyAttributeHelper(PropertyInfo property)
    {
        _property = property;
    }

    internal bool IsNotMapped()
    {
        var notMappedAttribute = _property.GetCustomAttribute<CronusAttributes.NotMappedAttribute>();
        return notMappedAttribute is not null;
    }

    internal string GetColumnName()
    {
        var columnAttribute = _property.GetCustomAttribute<CronusAttributes.ColumnAttribute>();
        return columnAttribute?.Name ?? _property.Name;
    }

    internal bool IsPrimaryKey()
    {
        var primaryKeyAttribute = _property.GetCustomAttribute<CronusAttributes.PrimaryKeyAttribute>();
        return primaryKeyAttribute is not null;
    }

    internal bool IsPrimaryKeyInteger()
    {
        return Type.GetTypeCode(_property.PropertyType) == TypeCode.Int32;
    }

    internal string? GetPropertyType()
    {
        var temp = _property.PropertyType;
        var mapper = new AllowedTypeMapper();
        return mapper.Map(temp).ToString();
    }

    internal JoinColumnAttribute? GetJoinColumn()
    {
        return _property.GetCustomAttribute<JoinColumnAttribute>();
    }
}
