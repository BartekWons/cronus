using Cronus.Interfaces;
using Cronus.Parser.Queries;

namespace Cronus.Parser.QueryExecutors
{
    internal class QueryExecutorFactory
    {
        private readonly IDbAdapter _db;

        public QueryExecutorFactory(IDbAdapter db)
        {
            _db = db;
        }

        public async Task<object?> ExecuteAsync(IQuery executor)
        {
            return executor switch
            {
                SelectQuery sq => await new SelectQueryExecutor(_db).ExecuteAsync(sq),
                InsertQuery iq => await new InsertQueryExecutor(_db).ExecuteAsync(iq),
                UpdateQuery iq => await new UpdateQueryExecutor(_db).ExecuteAsync(iq),
                DeleteQuery iq => await new DeleteQueryExecutor(_db).ExecuteAsync(iq),
                _ => throw new NotSupportedException("Operation not supported")
            };
        }
    }
}
