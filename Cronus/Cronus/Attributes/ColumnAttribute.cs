namespace Cronus.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class ColumnAttribute(string name) : Attribute
{
    public string? Name { get; set; } = name;
}
