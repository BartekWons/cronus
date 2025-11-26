using Cronus.Interfaces;

namespace Cronus.Parser.QueryExecutors
{
    internal class SqlQueryExecutor : ISqlQueryExecutor
    {
        private readonly IQueryParser _parser;
        private readonly QueryExecutorFactory _factory;

        public SqlQueryExecutor(IDatabaseAdapter db)
        {
            _parser = new QueryParser();
            _factory = new QueryExecutorFactory(db);
        }

        public Task<object?> ExecuteAsync(string sql)
        {
            var query = _parser.ParseQuery(sql);
            return _factory.ExecuteAsync(query);
        }
    }
}
