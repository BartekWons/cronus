using Cronus.Interfaces;
using Cronus.Parser.Queries;

namespace Cronus.Parser.QueryExecutors
{
    internal class DeleteQueryExecutor : QueryExecutorBase, IQueryExecutor<DeleteQuery>
    {
        public DeleteQueryExecutor(IDbAdapter db) : base(db) { }

        public async Task<object?> ExecuteAsync(DeleteQuery query)
        {
            var rowsAffected = await Db.DeleteAsync(query.Table, query.Condition);
            return rowsAffected;
        }
    }
}
