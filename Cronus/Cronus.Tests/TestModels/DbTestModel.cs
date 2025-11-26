using Cronus.Attributes;

namespace Cronus.Tests.TestModels
{
    [Table(nameof(DbTestModel))]
    internal class DbTestModel
    {
        public int SomeValues { get; set; }
    }
}
