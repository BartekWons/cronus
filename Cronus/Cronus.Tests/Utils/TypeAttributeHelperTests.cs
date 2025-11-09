using Cronus.Exceptions;
using Cronus.Tests.TestModels;
using Cronus.Utils;

namespace Cronus.Tests.Utils
{
    [TestFixture]
    public class TypeAttributeHelperTests
    {
        [Test]
        public void LackOfTableAttribute()
        {
            var obj = new TypeAttributeTestModel.WithoutTableAttribute();
            var tableProp = new TypeAttributeHelper(obj.GetType());
            Assert.That(() => tableProp.GetTableName(), Throws.TypeOf<AttributeNotFoundException>());
        }

        [Test]
        public void ValidTableAttributeName()
        {
            var obj = new TypeAttributeTestModel.WithTableAttribute();
            var tableProp = new TypeAttributeHelper(obj.GetType());
            Assert.That(tableProp.GetTableName(), Is.EqualTo("With_Table_Attribute"));
        }
    }
}
