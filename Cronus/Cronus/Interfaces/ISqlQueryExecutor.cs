namespace Cronus.Interfaces
{
    internal interface ISqlQueryExecutor
    {
        Task<object?> ExecuteAsync(string sql);

    }
}
