using Newtonsoft.Json;

namespace Cronus.DataAccess.Model;

[JsonObject(MemberSerialization.OptIn)]
internal class ForeignKeyModel
{
    [JsonProperty]
    internal string Column { get; set; } = default!;
    [JsonProperty]
    internal string ReferencedTable { get; set; } = default!;
    [JsonProperty]
    internal string ReferencedColumn { get; set; } = default!;
    [JsonProperty]
    internal bool CascadeDelete { get; set; }
}
