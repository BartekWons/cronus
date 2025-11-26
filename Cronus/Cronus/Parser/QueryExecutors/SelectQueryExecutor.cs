using Cronus.Interfaces;
using Cronus.Parser.Queries;

namespace Cronus.Parser.QueryExecutors
{
    internal class SelectQueryExecutor : QueryExecutorBase, IQueryExecutor<SelectQuery>
    {
        public SelectQueryExecutor(IDatabaseAdapter db) : base(db) { }

        public async Task<object?> ExecuteAsync(SelectQuery query)
        {
            var rows = await Db.SelectAsync(query.Table, query.Columns, query.Condition);
            return rows;
        }
    }
}
