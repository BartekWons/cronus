using Cronus.Runtime;

namespace Cronus.Builders;

public interface IBuilder
{
    DbBuilder CreateBuilder();
    Task<Db> BuildRuntimeAsync();
}
