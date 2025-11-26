using Cronus.Interfaces;

namespace Cronus.Parser.Queries
{
    internal sealed record UpdateQuery(string Table, IReadOnlyDictionary<string, object?> ValuesToSet, ICondition Where) : IQuery
    {
    }
}
