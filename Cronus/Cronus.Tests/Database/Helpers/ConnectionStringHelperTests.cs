using Cronus.Utils;
using FluentAssertions;
using NUnit.Framework;

namespace Cronus.Database.Helpers.Tests;

[TestFixture()]
public class ConnectionStringHelperTests
{
    [TestCase("Valid_ConnectionString")]
    [TestCase("Valid12_ConnectionString34")]
    [TestCase("Valid12%^_ConnectionString*&#")]
    public void IsValidTest_ReturnsTrue(string connectionString)
    {
        var handler = new ConnectionStringHandler(connectionString);
        handler.IsValid().Should().BeTrue();
    }

    [TestCase("")]
    [TestCase("InvalidConnectionString")]
    [TestCase("_InvalidConnectionString")]
    [TestCase("InvalidConnectionString_")]
    [TestCase("Invalid_Connection_String")]
    public void IsValidTest_ReturnsFalse(string connectionString)
    {
        var handler = new ConnectionStringHandler(connectionString);
        handler.IsValid().Should().BeFalse();
    }

    [TestCase(null)]
    public void IsValidTest_ThrowsException(string connectionString)
    {
        var handler = new ConnectionStringHandler(connectionString);
        var act = () => handler.IsValid().Should();
        act.Should().Throw<ArgumentNullException>();
    }

    [Test()]
    public void GetDatabaseNameTest()
    {
        var handler = new ConnectionStringHandler("group_id");
        var name = handler.GetDatabaseName();
        name.Should().Be("group");
    }

    [Test()]
    public void GetDatabaseIdTest()
    {
        var handler = new ConnectionStringHandler("group_id");
        var name = handler.GetDatabaseId();
        name.Should().Be("id");
    }
}