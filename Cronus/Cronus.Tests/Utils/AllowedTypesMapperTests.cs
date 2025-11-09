using Cronus.Utils;
using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;

namespace Cronus.Tests.Utils
{
    [TestFixture()]
    public class AllowedTypesMapperTests
    {
        [TestCase(typeof(string), AllowedType.String)]
        [TestCase(typeof(bool), AllowedType.Boolean)]
        [TestCase(typeof(int), AllowedType.Integer)]
        [TestCase(typeof(long), AllowedType.Integer)]
        [TestCase(typeof(double), AllowedType.Double)]
        [TestCase(typeof(float), AllowedType.Double)]
        public void MapTest_AllowedTypesCasting(Type type, AllowedType expectedType)
        {
            var mapper = new AllowedTypeMapper();
            Assert.That(mapper.Map(type), Is.EqualTo(expectedType));

        }

        [TestCase(typeof(decimal))]
        [TestCase(typeof(byte))]
        [TestCase(typeof(DateTime))]
        [TestCase(typeof(byte))]
        [TestCase(typeof(byte[]))]
        public void MapTest_NotAllowedTypesCasting(Type type)
        {
            var mapper = new AllowedTypeMapper();
            var act = () => mapper.Map(type);
            Assert.That(() => mapper.Map(type), Throws.TypeOf<NotSupportedException>());
        }

        [TestCase("String", AllowedType.String)]
        [TestCase("Boolean", AllowedType.Boolean)]
        [TestCase("Integer", AllowedType.Integer)]
        [TestCase("Double", AllowedType.Double)]
        [TestCase("string", AllowedType.String)]
        [TestCase("boolean", AllowedType.Boolean)]
        [TestCase("integer", AllowedType.Integer)]
        [TestCase("double", AllowedType.Double)]
        [TestCase("bool", AllowedType.Boolean)]
        [TestCase("Bool", AllowedType.Boolean)]
        public void MapFromStringTest_ValidType(string type, AllowedType expectedType)
        {
            var mapper = new AllowedTypeMapper();
            Assert.That(mapper.MapFromString(type), Is.EqualTo(expectedType));
        }

        [TestCase("Decimal")]
        [TestCase("Byte")]
        [TestCase("DateTime")]
        public void MapFromStringTest_InvalidType(string type)
        {
            var mapper = new AllowedTypeMapper();
            var act = () => mapper.MapFromString(type);
            Assert.That(() => mapper.MapFromString(type), Throws.TypeOf<NotSupportedException>());
        }
    }
}