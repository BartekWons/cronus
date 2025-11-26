using Cronus.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cronus.Tests.TestModels
{
    internal class AttributeHelperTestModel
    {
        [PrimaryKey]
        public int ValidPrimaryKey { get; set; }

        [PrimaryKey]
        public string InvalidPrimaryKey { get; set; }

        [NotMapped]
        public string IgnoredProperty { get; set; }

        [Column]
        public int ColumnName { get; set; }

        [Column("custom_column")]
        public string ColumnWithCustomName { get; set; }

        public string RegularProperty { get; set; }

        [JoinColumn("JoinColumnTest")]
        public int JoinColumnId { get; set; }
    }
}
