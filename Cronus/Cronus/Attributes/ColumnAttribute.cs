namespace Cronus.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class ColumnAttribute : Attribute
{
    public string? Name { get; set; }

    public ColumnAttribute()
    {
        
    }

    public ColumnAttribute(string name)
    {
        Name = name;
    }
}
