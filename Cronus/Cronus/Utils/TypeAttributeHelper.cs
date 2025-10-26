using Cronus.Attributes;
using Cronus.Exceptions;
using System.Reflection;

namespace Cronus.Utils;

internal class TypeAttributeHelper
{
    private Type _type;

    public TypeAttributeHelper(Type type)
    {
        _type = type;
    }

    internal string? GetTableName()
    {
        var tableAttribute = _type.GetCustomAttribute<TableAttribute>();
        if (tableAttribute is null)
            throw new AttributeNotFoundException("Missing Table attribute");
        return tableAttribute.Name;
    }

    internal PropertyInfo? GetPrimaryKeyInfo()
    {
        return _type.GetProperties().FirstOrDefault(prop => prop.GetCustomAttribute<PrimaryKeyAttribute>() is not null);
    }
}
