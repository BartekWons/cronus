using Cronus.Database.Config;
using Cronus.Database.Model;
using Cronus.Exceptions;

namespace Cronus.Database;

public class Database
{
    internal DatabaseModel Model { get; set; } = new();
    internal DatabaseConfig Config { get; set; } = new();

    internal async Task Init()
    {
        if (String.IsNullOrEmpty(Config.ConnectionString))
            throw new InvalidConnectionStringException("Connection string is null or empty");
        var handler = new DatabaseModelFileHandler(Model);
        await handler.SaveAsync(Config.ConnectionString);
    }

}
