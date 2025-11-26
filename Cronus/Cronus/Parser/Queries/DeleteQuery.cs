using Cronus.Interfaces;

namespace Cronus.Parser.Queries
{
    internal sealed record DeleteQuery(string Table, ICondition? Condition) : IQuery
    {

    }
}
