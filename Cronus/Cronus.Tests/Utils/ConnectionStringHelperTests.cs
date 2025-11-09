using Cronus.Utils;
using NUnit.Framework;

namespace Cronus.Tests.Utils;

[TestFixture()]
public class ConnectionStringHelperTests
{
    [TestCase("Valid_ConnectionString")]
    [TestCase("Valid12_ConnectionString34")]
    [TestCase("Valid12%^_ConnectionString*&#")]
    public void IsValidTest_ReturnsTrue(string connectionString)
    {
        var handler = new ConnectionStringHandler(connectionString);
        Assert.That(handler.IsValid(), Is.True);
    }

    [TestCase("")]
    [TestCase("InvalidConnectionString")]
    [TestCase("_InvalidConnectionString")]
    [TestCase("InvalidConnectionString_")]
    [TestCase("Invalid_Connection_String")]
    public void IsValidTest_ReturnsFalse(string connectionString)
    {
        var handler = new ConnectionStringHandler(connectionString);
        Assert.That(handler.IsValid(), Is.False);
    }

    [TestCase(null)]
    public void IsValidTest_ThrowsException(string connectionString)
    {
        var handler = new ConnectionStringHandler(connectionString);
        Assert.That(() => handler.IsValid(), Throws.TypeOf<ArgumentNullException>());
    }

    [Test()]
    public void GetDatabaseNameTest()
    {
        var handler = new ConnectionStringHandler("group_id");
        Assert.That(handler.GetDatabaseName(), Is.EqualTo("group"));
    }

    [Test()]
    public void GetDatabaseIdTest()
    {
        var handler = new ConnectionStringHandler("group_id");
        var name = handler.GetDatabaseId();
        Assert.That(handler.GetDatabaseId(), Is.EqualTo("id"));
    }
}