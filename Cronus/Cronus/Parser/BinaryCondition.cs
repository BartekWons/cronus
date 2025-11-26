using Cronus.Interfaces;

namespace Cronus.Parser
{
    internal sealed class BinaryCondition : ICondition
    {
        internal string Left { get; init; }
        internal string Operator { get; init; }
        internal object? Right { get; init; }
    }
}
