using Cronus.Interfaces;
using Cronus.Parser.Queries;
using Cronus.Parser.QueryExecutors;
using Moq;

namespace Cronus.Tests.QueryExecutors
{
    [TestFixture]
    public class QueryExecutorFactoryTests
    {
        [Test]
        public async Task ExecuteAsync_SelectQuery_ShouldCallSelectAsyncAndReturnRows()
        {
            var dbAdapterMock = new Mock<IDbAdapter>();

            var expectedRows = new List<IDictionary<string, object?>>
            {
                new Dictionary<string, object?> { ["UserId"] = 4, ["Name"] = "Tom" },
                new Dictionary<string, object?> { ["UserId"] = 5, ["Name"] = "Jerry" }
            };

            dbAdapterMock.Setup(db => db.SelectAsync(
                "Users", 
                It.IsAny<IReadOnlyList<string>>(),
                It.IsAny<ICondition?>()))
                .ReturnsAsync(expectedRows);

            var factory = new QueryExecutorFactory(dbAdapterMock.Object);

            var query = new SelectQuery(
                "Users",
                ["*"],
                null);

            var result = await factory.ExecuteAsync(query);

            var rows = result as IReadOnlyList<IDictionary<string, object?>>;

            Assert.That(rows, Is.Not.Null);
            Assert.That(rows!.Count, Is.EqualTo(2));
            Assert.That(rows[0]["UserId"], Is.EqualTo(4));
            Assert.That(rows[0]["Name"], Is.EqualTo("Tom"));
        }

        [Test]
        public async Task ExecuteAsync_InsertQuery_ShouldCallInsertAsync()
        {
            var dbAdapterMock = new Mock<IDbAdapter>();

            dbAdapterMock
                .Setup(db => db.InsertAsync(
                    It.IsAny<string>(),
                    It.IsAny<IReadOnlyDictionary<string, object?>>()))
                .Returns(Task.FromResult(true));

            var factory = new QueryExecutorFactory(dbAdapterMock.Object);

            var values = new Dictionary<string, object?>
            {
                ["UserId"] = 99,
                ["Name"] = "Inserted"
            };

            var query = new InsertQuery("Users", values);

            var result = await factory.ExecuteAsync(query);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task ExecuteAsync_UpdateQuery_ShouldCallUpdateAsyncAndReturnCount()
        {
            var dbAdapterMock = new Mock<IDbAdapter>();

            dbAdapterMock
                .Setup(db => db.UpdateAsync(
                    "Users",
                    It.IsAny<IReadOnlyDictionary<string, object?>>(),
                    It.IsAny<ICondition?>()))
                .ReturnsAsync(3);

            var factory = new QueryExecutorFactory(dbAdapterMock.Object);

            var values = new Dictionary<string, object?>
            {
                ["Age"] = 30
            };

            var query = new UpdateQuery("Users", values, null);

            var result = await factory.ExecuteAsync(query);

            Assert.That(result, Is.EqualTo(3));
        }

        [Test]
        public async Task ExecuteAsync_DeleteQuery_ShouldCallDeleteAsyncAndReturnCount()
        {
            var dbAdapterMock = new Mock<IDbAdapter>();

            dbAdapterMock
                .Setup(db => db.DeleteAsync(
                    "Users",
                    It.IsAny<ICondition?>()))
                .ReturnsAsync(1);

            var factory = new QueryExecutorFactory(dbAdapterMock.Object);

            var query = new DeleteQuery("Users", null);

            var result = await factory.ExecuteAsync(query);

            Assert.That(result, Is.EqualTo(1));
        }
    }
}
