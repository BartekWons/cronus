using Cronus.Interfaces;
using Cronus.Parser.QueryExecutors;
using Moq;

namespace Cronus.Tests.QueryExecutors
{
    [TestFixture]
    public class SqlQueryExecutorTests
    {
        [TestCase(@"SELECT * FROM Users")]
        [TestCase(@"INSERT INTO Users (UserId, Name) VALUES (1, 'Tom')")]
        [TestCase(@"UPDATE Users SET Name = 'Tom' WHERE UserId = 1")]
        [TestCase(@"DELETE FROM Users WHERE UserId = 1")]
        public async Task ExecuteAsync_ValidSql_ShouldNotThrow(string sql)
        {
            var dbAdapterMock = new Mock<IDbAdapter>();

            dbAdapterMock.Setup(db => db.SelectAsync(
                It.IsAny<string>(),
                It.IsAny<IReadOnlyList<string>>(),
                It.IsAny<ICondition?>()))
                .ReturnsAsync(new List<IDictionary<string, object?>>());

            dbAdapterMock.Setup(db => db.InsertAsync(
                It.IsAny<string>(),
                It.IsAny<IReadOnlyDictionary<string, object?>>()))
                .Returns(Task.FromResult(true));

            dbAdapterMock.Setup(db => db.UpdateAsync(
                It.IsAny<string>(),
                It.IsAny<IReadOnlyDictionary<string, object?>>(),
                It.IsAny<ICondition?>()))
                .Returns(Task.FromResult(0));

            dbAdapterMock.Setup(db => db.DeleteAsync(
                It.IsAny<string>(),
                It.IsAny<ICondition?>()))
                .Returns(Task.FromResult(0));

            var sut = new SqlQueryExecutor(dbAdapterMock.Object);

            Assert.DoesNotThrowAsync(async () => await sut.ExecuteAsync(sql));
        }

        [TestCase(@"SELECT FROM Users")]
        [TestCase(@"INSERT Users VALUES (1, 'Tom')")]
        [TestCase(@"UPDATE SET Name = 'Tom'")]
        [TestCase(@"DELETE Users WHERE Id = 1")]
        public void ExecuteAsync_InvalidSql_ShouldThrowInvalidOperationException(string sql)
        {
            var dbAdapterMock = new Mock<IDbAdapter>();
            var sut = new SqlQueryExecutor(dbAdapterMock.Object);

            Assert.ThrowsAsync<InvalidOperationException>(async () => await sut.ExecuteAsync(sql));
        }
    }
}
