namespace Cronus.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class JoinColumnAttribute(string name) : Attribute
{
    public string? Name { get; set; } = name;
}