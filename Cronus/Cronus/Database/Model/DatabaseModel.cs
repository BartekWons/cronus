using Newtonsoft.Json;

namespace Cronus.Database.Model;

[JsonObject(MemberSerialization.OptIn)]
internal class DatabaseModel
{
    [JsonProperty]
    internal List<TableModel> Tables { get; set; } = [];
}
