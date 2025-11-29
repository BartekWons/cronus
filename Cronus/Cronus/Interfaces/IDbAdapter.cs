namespace Cronus.Interfaces
{
    internal interface IDbAdapter
    {
        Task<bool> InsertAsync(string table, IReadOnlyDictionary<string, object>? values);
        Task<int> UpdateAsync(string table, IReadOnlyDictionary<string, object>? values, ICondition? whereCondition);
        Task<int> DeleteAsync(string table, ICondition? where);
        Task<IReadOnlyList<IDictionary<string, object?>>> SelectAsync(string table, IReadOnlyList<string> columns, ICondition? whereCondition);
    }
}
