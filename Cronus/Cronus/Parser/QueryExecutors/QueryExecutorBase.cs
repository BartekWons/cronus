using Cronus.Interfaces;

namespace Cronus.Parser.QueryExecutors
{
    internal class QueryExecutorBase
    {
        protected readonly IDbAdapter Db;

        public QueryExecutorBase(IDbAdapter db)
        {
            Db = db;
        }
    }
}
