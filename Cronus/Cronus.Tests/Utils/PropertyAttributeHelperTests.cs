using Cronus.Tests.TestModels;
using Cronus.Utils;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Internal;
using System.Reflection;

namespace Cronus.Tests.Utils
{
    [TestFixture]
    public class PropertyAttributeHelperTests
    {
        private PropertyInfo? GetProperty(string name)
        {
            return typeof(AttributeHelperTestModel).GetProperty(name);
        }

        [Test]
        public void ValidPrimaryKeyType()
        {
            var prop = GetProperty(nameof(AttributeHelperTestModel.ValidPrimaryKey));
            var helper = new PropertyAttributeHelper(prop);
            helper.Invoking(h => h.IsPrimaryKeyInteger()).Should().NotThrow();
        }

        [Test]
        public void IsPrimaryKey()
        {
            var prop = GetProperty(nameof(AttributeHelperTestModel.ValidPrimaryKey));
            var helper = new PropertyAttributeHelper(prop);
            helper.IsPrimaryKey().Should().BeTrue();
        }

        [Test]
        public void IsNotPrimaryKey()
        {
            var prop = GetProperty(nameof(AttributeHelperTestModel.IgnoredProperty));
            var helper = new PropertyAttributeHelper(prop);
            helper.IsPrimaryKey().Should().BeFalse();
        }

        [Test]
        public void InvalidPrimaryKeyType()
        {
            var prop = GetProperty(nameof(AttributeHelperTestModel.InvalidPrimaryKey));
            var helper = new PropertyAttributeHelper(prop);
            helper.IsPrimaryKeyInteger().Should().BeFalse();
        }

        [Test]
        public void IsNotMapped()
        {
            var prop = GetProperty(nameof(AttributeHelperTestModel.IgnoredProperty));
            var helper = new PropertyAttributeHelper(prop);
            helper.IsNotMapped().Should().BeTrue();
        }

        [Test]
        public void NormalTableName()
        {
            var prop = GetProperty(nameof(AttributeHelperTestModel.ColumnName));
            var helper = new PropertyAttributeHelper(prop);
            helper.GetColumnName().Should().Be(nameof(AttributeHelperTestModel.ColumnName));
        }

        [Test]
        public void CustomTableName()
        {
            var prop = GetProperty(nameof(AttributeHelperTestModel.ColumnWithCustomName));
            var helper = new PropertyAttributeHelper(prop);
            helper.GetColumnName().Should().Be("custom_column");
        }
    }
}
