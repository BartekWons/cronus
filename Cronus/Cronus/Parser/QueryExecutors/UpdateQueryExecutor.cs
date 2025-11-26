using Cronus.Interfaces;
using Cronus.Parser.Queries;

namespace Cronus.Parser.QueryExecutors
{
    internal class UpdateQueryExecutor : QueryExecutorBase, IQueryExecutor<UpdateQuery>
    {
        public UpdateQueryExecutor(IDatabaseAdapter db) : base(db) { }

        public async Task<object?> ExecuteAsync(UpdateQuery query)
        {
            var rowsAffected = await Db.UpdateAsync(query.Table, query.ValuesToSet, query.Where);
            return rowsAffected;
        }
    }
}
