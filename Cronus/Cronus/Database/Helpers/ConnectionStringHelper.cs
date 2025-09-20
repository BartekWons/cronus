using System.Text.RegularExpressions;

namespace Cronus.Database.Helpers;

internal partial class ConnectionStringHandler
{
    private string _connectionString;

    internal ConnectionStringHandler(string connectionString)
    {
        _connectionString = connectionString;
    }

    [GeneratedRegex(@"^([^_]+)_([^_]+)$", RegexOptions.Compiled)]
    private static partial Regex ConnectionStringSplitter();

    internal bool IsValid()
    {
        return ConnectionStringSplitter().Match(_connectionString).Success;
    }

    internal string GetDatabaseName()
    {
        var match = ConnectionStringSplitter().Match(_connectionString);
        return match.Success ? match.Groups[1].Value : string.Empty;
    }

    internal string GetDatabaseId()
    {
        var match = ConnectionStringSplitter().Match(_connectionString);
        return match.Success ? match.Groups[2].Value : string.Empty;
    }
}