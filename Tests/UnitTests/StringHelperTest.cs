using ClassBuilderGenerator.Core;

using FluentAssertions;

using Xunit;

namespace UnitTests
{
    public class StringHelperTest
    {
        [Theory]
        [InlineData("System.int", "int")]
        [InlineData("System.DateTime", "DateTime")]
        [InlineData("Custom.Namespace.CustomObject", "CustomObject")]
        [InlineData("System.Collections.Generic.List<int>", "List<int>")]
        [InlineData("System.Collections.Generic.List<System.Datetime>", "List<Datetime>")]
        [InlineData("System.Collections.Generic.List<Custom.Namespace.CustomObject>", "List<CustomObject>")]
        [InlineData("System.Collections.Generic.List<System.Collections.Generic.List<Custom.Namespace.CustomObject>>", "List<List<CustomObject>>")]
        [InlineData("System.Collections.Generic.List<System.Collections.Generic.List<int>>", "List<List<int>>")]
        [InlineData("System.Collections.Generic.List<System.Collections.Generic.List<System.int>>", "List<List<int>>")]
        [InlineData("System.Collections.Generic.Dictionary<int, string>", "Dictionary<int, string>")]
        [InlineData("System.Collections.Generic.Dictionary<System.int, System.string>", "Dictionary<int, string>")]
        [InlineData("System.Collections.Generic.Dictionary<System.int, Custom.Namespace.CustomObject>", "Dictionary<int, CustomObject>")]
        [InlineData("System.Collections.Generic.Dictionary<System.int, System.Collections.Generic.List<int>", "Dictionary<int, List<int>>")]
        [InlineData("System.Collections.Generic.Dictionary<System.int, System.Collections.Generic.List<Custom.Namespace.CustomObject>", "Dictionary<int, List<CustomObject>>")]
        public void ShouldExecuteRemoveNamespaceSuccessfully(string input, string expectedResult)
        {
            var result = input.RemoveNamespace();

            result.Should().Be(expectedResult);
        }

        [Theory]
        [InlineData("System.int", "int", 5)]
        [InlineData("System.DateTime", "DateTime", 5)]
        [InlineData("Custom.Namespace.CustomObject", "CustomObject", 6)]
        [InlineData("System.Collections.Generic.List<int>", "List<int>", 5)]
        [InlineData("System.Collections.Generic.List<System.Datetime>", "List<Datetime>", 5)]
        [InlineData("System.Collections.Generic.List<Custom.Namespace.CustomObject>", "List<CustomObject>", 6)]
        [InlineData("System.Collections.Generic.List<System.Collections.Generic.List<Custom.Namespace.CustomObject>>", "List<List<CustomObject>>", 6)]
        [InlineData("System.Collections.Generic.List<System.Collections.Generic.List<int>>", "List<List<int>>", 5)]
        [InlineData("System.Collections.Generic.List<System.Collections.Generic.List<System.int>>", "List<List<int>>", 5)]
        [InlineData("System.Collections.Generic.Dictionary<int, string>", "Dictionary<int, string>", 5)]
        [InlineData("System.Collections.Generic.Dictionary<System.int, System.string>", "Dictionary<int, string>", 5)]
        [InlineData("System.Collections.Generic.Dictionary<System.int, Custom.Namespace.CustomObject>", "Dictionary<int, CustomObject>", 6)]
        [InlineData("System.Collections.Generic.Dictionary<System.int, System.Collections.Generic.List<int>", "Dictionary<int, List<int>>", 5)]
        [InlineData("System.Collections.Generic.Dictionary<System.int, System.Collections.Generic.List<Custom.Namespace.CustomObject>", "Dictionary<int, List<CustomObject>>", 6)]
        public void ShouldExecuteRemoveNamespaceWithClassInformationSuccessfully(string input, string expectedResult, int expectedUsingCount)
        {
            var classInformation = new ClassInformation();

            var result = input.RemoveNamespace(classInformation);

            result.Should().Be(expectedResult);
            classInformation.Usings.Should().HaveCount(expectedUsingCount);
        }

        [Theory]
        [InlineData("Location_ID", "locationId")]
        [InlineData("testLEFTSide", "testLeftSide")]
        public void ShouldExecuteToCamelCaseSuccessfully(string input, string expectedResult)
        {
            var result = input.ToCamelCase();

            result.Should().Be(expectedResult);
        }
    }
}