using Cronus.Attributes;

namespace Cronus.Tests.TestModels
{
    internal class TypeAttributeTestModel
    {
        [PrimaryKey]
        public int TypeId { get; set; }

        [Table("With_Table_Attribute")]
        public class WithTableAttribute
        {
            public int NullId { get; set; }
        }

        public class WithoutTableAttribute;
    }
}
