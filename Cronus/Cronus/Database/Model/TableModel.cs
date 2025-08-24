namespace Cronus.Database.Model;

internal class TableModel
{
    internal string Name { get; set; } = default!;
    internal List<ColumnModel>? Columns { get; set; }
}
