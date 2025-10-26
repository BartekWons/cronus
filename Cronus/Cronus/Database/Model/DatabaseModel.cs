using Newtonsoft.Json;

namespace Cronus.Database.Model;

[JsonObject(MemberSerialization.OptIn)]
internal class DatabaseModel
{
    [JsonProperty]
    internal List<TableModel> TablesSchema { get; set; } = [];
    [JsonProperty]
    internal Dictionary<string, List<Dictionary<string, object?>>> Data { get; set; } = [];
}
