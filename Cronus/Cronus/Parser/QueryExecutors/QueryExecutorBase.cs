using Cronus.Interfaces;

namespace Cronus.Parser.QueryExecutors
{
    internal class QueryExecutorBase
    {
        protected readonly IDatabaseAdapter Db;

        public QueryExecutorBase(IDatabaseAdapter db)
        {
            Db = db;
        }
    }
}
