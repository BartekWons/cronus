using Cronus.Exceptions;
using Cronus.Utils;

namespace Cronus.Database;

internal class DatabaseConfig
{
    internal string ConnectionString { get; private set; } = default!;
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
