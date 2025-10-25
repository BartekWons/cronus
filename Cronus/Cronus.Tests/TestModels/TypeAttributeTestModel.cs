using Cronus.Attributes;

namespace Cronus.Tests.TestModels
{
    internal class TypeAttributeTestModel
    {
        [Table("With_Table_Attribute")]
        public class WithTableAttribute;

        public class WithoutTableAttribute;
    }
}
