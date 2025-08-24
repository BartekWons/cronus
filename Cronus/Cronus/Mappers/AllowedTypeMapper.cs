using Cronus.Types;

namespace Cronus.Mappers;

internal class AllowedTypeMapper : IMapper<AllowedType, Type>
{
    public AllowedType Map(Type value)
    {
        return value switch
        {
            var t when t == typeof(string) => AllowedType.String,
            var t when t == typeof(int) => AllowedType.Integer,
            var t when t == typeof(long) => AllowedType.Integer,
            var t when t == typeof(double) => AllowedType.Double,
            var t when t == typeof(float) => AllowedType.Double,
            var t when t == typeof(bool) => AllowedType.Boolean,
            _ => throw new NotSupportedException($"Type {value} not supported")
        };
    }

    internal AllowedType MapFromString(string type)
    {
        return type.ToLower() switch
        {
            "string" => AllowedType.String,
            "integer" => AllowedType.Integer,
            "double" => AllowedType.Double,
            "boolean" => AllowedType.Boolean,
            "bool" => throw new NotSupportedException("Type bool is not supported, use Boolean instead"),
            _ => throw new NotSupportedException($"Type {type} not supported")
        };
    }
}
