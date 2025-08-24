using Cronus.Exceptions;

namespace Cronus.Database.Config;

internal class DatabaseConfig
{
    internal string ConnectionString { get; set; } = default!;
    internal string Name { get; private set; } = default!;
    internal string Id { get; private set; } = default!;

    internal void AddConnectionString(string connectionString)
    {
        var handler = new ConnectionStringHandler(connectionString);
        if (!handler.IsValid())
            throw new InvalidConnectionStringException();

        ConnectionString = connectionString;
        Name = handler.GetDatabaseName();
        Id = handler.GetDatabaseId();
    }
}
