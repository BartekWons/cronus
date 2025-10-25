using Cronus.Utils;
using FluentAssertions;
using NUnit.Framework;

namespace Cronus.Mappers.Tests
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
            mapper.Map(type).Should().Be(expectedType);

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
            act.Should().Throw<NotSupportedException>();
        }

        [TestCase("String", AllowedType.String)]
        [TestCase("Boolean", AllowedType.Boolean)]
        [TestCase("Integer", AllowedType.Integer)]
        [TestCase("Double", AllowedType.Double)]
        [TestCase("string", AllowedType.String)]
        [TestCase("boolean", AllowedType.Boolean)]
        [TestCase("integer", AllowedType.Integer)]
        [TestCase("double", AllowedType.Double)]
        public void MapFromStringTest_ValidType(string type, AllowedType expectedType)
        {
            var mapper = new AllowedTypeMapper();
            mapper.MapFromString(type).Should().Be(expectedType);
        }

        [TestCase("Decimal")]
        [TestCase("Byte")]
        [TestCase("DateTime")]
        public void MapFromStringTest_InvalidType(string type)
        {
            var mapper = new AllowedTypeMapper();
            var act = () => mapper.MapFromString(type);
            act.Should().Throw<NotSupportedException>();
        }

        [Test()]
        public void MapFromStringTest_BoolType()
        {
            var mapper = new AllowedTypeMapper();
            var act = () => mapper.MapFromString("bool");
            act.Should().Throw<NotSupportedException>().WithMessage("Type bool is not supported, use Boolean instead");
        }
    }
}