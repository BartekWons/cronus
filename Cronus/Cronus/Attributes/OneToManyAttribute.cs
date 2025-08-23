namespace Cronus.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class OneToManyAttribute(string mappedBy) : Attribute
{
    public string MappedBy { get; set; } = mappedBy;
}

