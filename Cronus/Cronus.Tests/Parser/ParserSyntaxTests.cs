using Cronus.Parser;
using Cronus.Parser.Queries;
using NUnit.Framework.Internal;
using System.ComponentModel;

namespace Cronus.Tests.Parser
{
    [TestFixture]
    public class ParserSyntaxTests
    {
        private QueryParser _parser;
        [SetUp]
        public void SetUp()
        {
            _parser = new QueryParser();
        }

        [TestCase("SELECT * FROM Users", typeof(SelectQuery))]
        [TestCase("SELECT Name FROM Users", typeof(SelectQuery))]
        [TestCase("SELECT UserId FROM Users", typeof(SelectQuery))]
        [TestCase("SELECT * FROM Users WHERE Age == 6", typeof(SelectQuery))]
        [TestCase("   SELECT  *   FROM    Users     WHERE Age     == 6", typeof(SelectQuery))]
        [TestCase("   SELECT  Age   FROM    Users     WHERE Age     == 6", typeof(SelectQuery))]
        [TestCase("   SELECT  UserId   FROM    Users     WHERE Age     ==  6", typeof(SelectQuery))]
        [TestCase("   SELECT  UserId   FROM    Users     WHERE Age     =  6", typeof(SelectQuery))]
        [TestCase("INSERT INTO Users (Id, Name) VALUES (1, \"John\")", typeof(InsertQuery))]
        [TestCase("  INSERT  INTO  Users (Id) VALUES (123)", typeof(InsertQuery))]
        [TestCase("INSERT INTO Orders (OrderId, Amount) VALUES (1, 999)", typeof(InsertQuery))]
        [TestCase("UPDATE Users SET Name = \"John\" WHERE Id = 1", typeof(UpdateQuery))]
        [TestCase("UPDATE Users SET Age = 30", typeof(UpdateQuery))]
        [TestCase("  UPDATE   Users  SET  Name = \"John\", Age = 40  WHERE   Id =  1", typeof(UpdateQuery))]
        [TestCase("DELETE FROM Users WHERE Id = 1", typeof(DeleteQuery))]
        [TestCase("  DELETE   FROM   Users   WHERE   Age =  10", typeof(DeleteQuery))]
        [TestCase("DELETE FROM Users", typeof(DeleteQuery))]
        public void ParseQuery_WithValidSql_ReturnsExpectedQueryType(string sql, Type expectedType)
        {
            var result = _parser.ParseQuery(sql);

            Assert.That(result, Is.TypeOf(expectedType));
        }

        [TestCase("SELECT FROM Users")]
        [TestCase("SELECT * Users")]
        [TestCase("SELECT 2312* Users")]
        [TestCase("SELECT Id Name FROM Users")]
        [TestCase("SELECT * FROM")]
        [TestCase("SELECT Id WHERE Age = 5")]
        [TestCase("select Id WHERE Age = 5")]
        [TestCase("INSERT Users (Id) VALUES (1)")]
        [TestCase("INSERT INTO Users Id VALUES (1)")]
        [TestCase("INSERT INTO Users (Id) VALUE (1)")]
        [TestCase("INSERT INTO (Id) VALUES (1)")]
        [TestCase("UPDATE SET Name = \"John\" WHERE Id = 1")]
        [TestCase("UPDATE Users Name = \"John\" WHERE Id = 1")]
        [TestCase("UPDATE Users SET WHERE Id = 1")]
        [TestCase(@"UPDATE Users Name ""John""")]
        [TestCase("DELETE Users WHERE 32 Id = 1")]
        [TestCase("DELETE FROM")]
        [TestCase("")]
        public void ParseQuery_WithInvalidSql_ThrowsInvalidOperationException(string sql)
        {
            Assert.Throws<InvalidOperationException>(() => _parser.ParseQuery(sql));
        }
    }
}
