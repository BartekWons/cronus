using Cronus.Runtime;

namespace Cronus.Builders;

public interface IBuilder
{
    Task<Db> BuildRuntimeAsync();
}
