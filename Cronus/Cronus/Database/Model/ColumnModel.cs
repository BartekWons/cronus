namespace Cronus.Database.Model;

internal class ColumnModel
{
    internal string Name { get; set; } = default!;
    internal string Type { get; set; } = default!;
    internal bool IsPrimaryKey { get; set; }
}
