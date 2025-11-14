using Cronus.DataAccess;
using Cronus.Exceptions;

namespace Cronus.Tests.Utils
{
    [TestFixture]
    public class DatabaseConfigTests
    {
        [TestCase("Hello_Id", "Hello", "Id")]
        [TestCase("Hello3_12", "Hello3", "12")]
        [TestCase("Hello#_$%", "Hello#", "$%")]
        public void AddConnectionString_ValidConnString_SetsProperties(string connectionString, string expectedName, string expectedId)
        {
            var config = new DatabaseConfig();
            config.AddConnectionString(connectionString);

            Assert.Multiple(() =>
            {
                Assert.That(config.ConnectionString, Is.EqualTo(connectionString));
                Assert.That(config.Name, Is.EqualTo(expectedName));
                Assert.That(config.Id, Is.EqualTo(expectedId));
            });
        }

        [TestCase("Witam_")]
        [TestCase("Witam3")]
        [TestCase("_awda")]
        public void AddConnectionString_InvalidConnString_ThrowsException(string connectionString)
        {
            var config = new DatabaseConfig();
            Assert.That(() => config.AddConnectionString(connectionString), Throws.TypeOf<InvalidConnectionStringException>());
        }

        [Test]
        public void AddConnectionString_NullConnString_ThrowsException()
        {
            var config = new DatabaseConfig();
            Assert.That(() => config.AddConnectionString(null), Throws.TypeOf<ArgumentNullException>());
        }
    }
}
