using Cronus.Interfaces;

namespace Cronus.Parser.Queries
{
    internal sealed record InsertQuery(string Table, IReadOnlyDictionary<string, object?> Values) : IQuery
    {
    }
}
