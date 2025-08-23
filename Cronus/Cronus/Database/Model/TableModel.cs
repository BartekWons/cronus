namespace Cronus.Database.Model;

public class TableModel
{
    public string Name { get; set; } = default!;
    public List<ColumnModel>? Columns { get; set; }
}
