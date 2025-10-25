namespace Cronus.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class ManyToManyAttribute(string mappedBy) : Attribute
{
    public string? MappedBy { get; set; } = mappedBy;
}
