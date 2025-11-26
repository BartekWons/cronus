using Newtonsoft.Json;

namespace Cronus.DataAccess.Model;

[JsonObject(MemberSerialization.OptIn)]
internal class TableModel
{
    [JsonProperty]
    internal string Name { get; set; } = default!;
    [JsonProperty]
    internal List<ColumnModel>? Columns { get; set; }
    [JsonProperty]
    internal List<ForeignKeyModel> ForeignKeys { get; set; } = [];
}
