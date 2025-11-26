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

        [Test]
        public void GetPrimaryKeyInfo_ReturnsPrimaryKey()
        {
            var obj = new TypeAttributeTestModel();
            var helper = new TypeAttributeHelper(obj.GetType());
            var pkProp = helper.GetPrimaryKeyInfo();
            Assert.That(pkProp, Is.Not.Null);
        }

        [Test]
        public void GetPrimaryKeyInfo_ReturnsNull()
        {
            var obj = new TypeAttributeTestModel.WithTableAttribute();
            var helper = new TypeAttributeHelper(obj.GetType());
            var pkProp = helper.GetPrimaryKeyInfo();
            Assert.That(pkProp, Is.Null);
        }
    }
}
