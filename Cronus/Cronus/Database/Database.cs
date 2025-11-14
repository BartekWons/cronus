using Cronus.DataAccess.Model;

namespace Cronus.Database;

public class Database
{
    internal DatabaseModel Model { get; set; } = new();
    internal DatabaseConfig Config { get; set; } = new();
}
