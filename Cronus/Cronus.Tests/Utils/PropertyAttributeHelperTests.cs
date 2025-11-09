using Cronus.Tests.TestModels;
using Cronus.Utils;
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
            Assert.That(() => helper.IsPrimaryKeyInteger(), Throws.Nothing);
        }

        [Test]
        public void IsPrimaryKey()
        {
            var prop = GetProperty(nameof(AttributeHelperTestModel.ValidPrimaryKey));
            var helper = new PropertyAttributeHelper(prop);
            Assert.That(helper.IsPrimaryKey(), Is.True);
        }

        [Test]
        public void IsNotPrimaryKey()
        {
            var prop = GetProperty(nameof(AttributeHelperTestModel.IgnoredProperty));
            var helper = new PropertyAttributeHelper(prop);
            Assert.That(helper.IsPrimaryKey(), Is.False);
        }

        [Test]
        public void InvalidPrimaryKeyType()
        {
            var prop = GetProperty(nameof(AttributeHelperTestModel.InvalidPrimaryKey));
            var helper = new PropertyAttributeHelper(prop);
            Assert.That(helper.IsPrimaryKeyInteger(), Is.False);
        }

        [Test]
        public void IsNotMapped()
        {
            var prop = GetProperty(nameof(AttributeHelperTestModel.IgnoredProperty));
            var helper = new PropertyAttributeHelper(prop);
            Assert.That(helper.IsNotMapped(), Is.True);
        }

        [Test]
        public void NormalTableName()
        {
            var prop = GetProperty(nameof(AttributeHelperTestModel.ColumnName));
            var helper = new PropertyAttributeHelper(prop);
            Assert.That(helper.GetColumnName(), Is.EqualTo(nameof(AttributeHelperTestModel.ColumnName)));
        }

        [Test]
        public void CustomTableName()
        {
            var prop = GetProperty(nameof(AttributeHelperTestModel.ColumnWithCustomName));
            var helper = new PropertyAttributeHelper(prop);
            Assert.That(helper.GetColumnName(), Is.EqualTo("custom_column"));
        }
    }
}
