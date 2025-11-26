using Newtonsoft.Json;

namespace Cronus.DataAccess.Model;

[JsonObject(MemberSerialization.OptIn)]
internal class ColumnModel
{
    [JsonProperty]
    internal string Name { get; set; } = default!;
    [JsonProperty]
    internal string Type { get; set; } = default!;
    [JsonProperty]
    internal bool IsPrimaryKey { get; set; }
}
