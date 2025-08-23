namespace Cronus.Database.Model;

public class ColumnModel
{
    public string Name { get; set; } = default!;
    public string Type { get; set; } = default!;
    public bool IsPrimaryKey { get; set; }
}
