using Cronus.Interfaces;

namespace Cronus.Parser.Queries
{
    internal record SelectQuery(string Table, IReadOnlyList<string> Columns, ICondition? Condition) : IQuery
    {
    }
}
