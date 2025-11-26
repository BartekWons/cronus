using Cronus.Interfaces;
using Cronus.Parser.Queries;

namespace Cronus.Parser.QueryExecutors
{
    internal class InsertQueryExecutor : QueryExecutorBase, IQueryExecutor<InsertQuery>
    {
        public InsertQueryExecutor(IDatabaseAdapter db) : base(db) { }

        public async Task<object?> ExecuteAsync(InsertQuery query)
        {
            await Db.InsertAsync(query.Table, query.Values!);
            return null;
        }
    }
}
