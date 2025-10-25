namespace Cronus.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class TableAttribute(string name) : Attribute
{
    public string? Name { get; set; } = name;
}
