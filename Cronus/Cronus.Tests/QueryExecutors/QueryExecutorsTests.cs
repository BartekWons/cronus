using Cronus.Interfaces;
using Cronus.Parser;
using Cronus.Parser.Queries;
using Cronus.Parser.QueryExecutors;
using Moq;

namespace Cronus.Tests.QueryExecutors
{
    [TestFixture]
    public class QueryExecutorsTests
    {
        [Test]
        public async Task ExecuteAsync_ShouldCallDeleteAsync_AndReturnRowsAffected()
        {
            var dbMock = new Mock<IDatabaseAdapter>();

            dbMock.Setup(db => db.DeleteAsync(
                "Users",
                It.IsAny<ICondition?>()))
                .ReturnsAsync(3);
            
            var executor = new DeleteQueryExecutor(dbMock.Object);

            var query = new DeleteQuery(
                Table: "Users",
                Condition: null);

            var result = await executor.ExecuteAsync(query);

            Assert.That(result, Is.EqualTo(3));
        }

        [Test]
        public async Task ExecuteAsync_ShouldCallInsertAsync_AndReturnNull()
        {
            var dbMock = new Mock<IDatabaseAdapter>();

            dbMock.Setup(db => db.InsertAsync(
                "Users",
                It.IsAny<IReadOnlyDictionary<string, object?>>()))
                .Returns(Task.CompletedTask);

            var executor = new InsertQueryExecutor(dbMock.Object);

            var values = new Dictionary<string, object?>
            {
                ["UserId"] = 10,
                ["Name"] = "Tom",
            };

            var query = new InsertQuery(
                Table: "Users",
                Values: values);

            var result = await executor.ExecuteAsync(query);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task ExecuteAsync_ShouldCallSelectAsync_AndReturnRows()
        {
            var dbMock = new Mock<IDatabaseAdapter>();

            var expectedRows = new List<IDictionary<string, object?>>
            {
                new Dictionary<string, object?> { ["UserId"] = 1, ["Name"] = "Tom" },
                new Dictionary<string, object?> { ["UserId"] = 2, ["Name"] = "Jerry" }
            };

            dbMock
                .Setup(db => db.SelectAsync(
                    "Users",
                    It.IsAny<IReadOnlyList<string>>(),
                    It.IsAny<ICondition?>()))
                .ReturnsAsync(expectedRows);

            var executor = new SelectQueryExecutor(dbMock.Object);

            var query = new SelectQuery(
                Table: "Users",
                Columns: new List<string> { "*" },
                Condition: null);

            var result = await executor.ExecuteAsync(query);

            Assert.That(result, Is.Not.Null);

            Assert.That(result, Is.InstanceOf<IReadOnlyList<IDictionary<string, object?>>>());

            var rows = (IReadOnlyList<IDictionary<string, object?>>)result!;

            Assert.That(rows.Count, Is.EqualTo(2));
            Assert.That(rows[0]["UserId"], Is.EqualTo(1));
            Assert.That(rows[0]["Name"], Is.EqualTo("Tom"));
            Assert.That(rows[1]["UserId"], Is.EqualTo(2));
            Assert.That(rows[1]["Name"], Is.EqualTo("Jerry"));
        }

        [Test]
        public async Task ExecuteAsync_ShouldCallUpdateAsync_AndReturnRowsAffected()
        {
            var dbMock = new Mock<IDatabaseAdapter>();

            dbMock
                .Setup(db => db.UpdateAsync(
                    "Users",
                    It.IsAny<IReadOnlyDictionary<string, object?>>(),
                    It.IsAny<ICondition?>()))
                .ReturnsAsync(2);

            var executor = new UpdateQueryExecutor(dbMock.Object);

            var valuesToSet = new Dictionary<string, object?>
            {
                ["Name"] = "Tom",
                ["Age"] = 30
            };

            var condition = new BinaryCondition
            {
                Left = "UserId",
                Operator = "=",
                Right = 10
            };

            var query = new UpdateQuery(
                Table: "Users",
                ValuesToSet: valuesToSet,
                Where: condition);

            var result = await executor.ExecuteAsync(query);

            Assert.That(result, Is.EqualTo(2));
        }
    }
}
