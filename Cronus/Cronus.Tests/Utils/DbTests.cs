using Cronus.DataAccess;
using Cronus.Runtime;
using Cronus.Tests.TestModels;

namespace Cronus.Tests.Utils
{
    [TestFixture]
    public class DbTests
    {

        [Test]
        public void Set_ReturnsDbObject()
        {
            var database = new Database();
            database.Model.TablesSchema.Add(new DataAccess.Model.TableModel
            {
                Name = nameof(DbTestModel),
            });
            var db = new Db(database);
            var result = db.Set<DbTestModel>();
            Assert.That(result, Is.InstanceOf<DbSet<DbTestModel>>());
        }
    }
}
