using Cronus.Exceptions;
using Cronus.Tests.TestModels;
using Cronus.Utils;
using FluentAssertions;
using NUnit.Framework;

namespace Cronus.Tests.Database.Helpers
{
    [TestFixture]
    public class TypeAttributeHelperTests
    {
        [Test]
        public void LackOfTableAttribute()
        {
            var obj = new TypeAttributeTestModel.WithoutTableAttribute();
            var tableProp = new TypeAttributeHelper(obj.GetType());
            tableProp.Invoking(p => p.GetTableName()).Should().Throw<AttributeNotFoundException>();
        }

        [Test]
        public void ValidTableAttributeName()
        {
            var obj = new TypeAttributeTestModel.WithTableAttribute();
            var tableProp = new TypeAttributeHelper(obj.GetType());
            tableProp.GetTableName().Should().Be("With_Table_Attribute");
        }
    }
}
