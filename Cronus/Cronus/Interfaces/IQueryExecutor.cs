namespace Cronus.Interfaces
{
    internal interface IQueryExecutor<in T> where T : IQuery
    {
        Task<object?> ExecuteAsync(T query);
    }
}
