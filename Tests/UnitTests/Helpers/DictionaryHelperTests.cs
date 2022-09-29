using ClassBuilderGenerator.Helpers;

using FluentAssertions;

using Xunit;

namespace UnitTests.Helpers
{
    public class DictionaryHelperTests
    {
        [Theory(DisplayName = "Should return the correct key of a dictionary")]
        [InlineData("Dictionary<int, string>", "int")]
        [InlineData("Dictionary<Guid, MyClass>", "Guid")]
        public void GetDictionaryKeyTypeTests(string dictionary, string expected)
        {
            // Arrange

            // Act
            var result = dictionary.GetDictionaryKeyType();

            // Assert
            result.Should().Be(expected);
        }

        [Theory(DisplayName = "Should return the correct value of a dictionary")]
        [InlineData("Dictionary<int, string>", "string")]
        [InlineData("Dictionary<Guid, MyClass>", "MyClass")]
        public void GetDictionaryValueTypeTests(string dictionary, string expected)
        {
            // Arrange

            // Act
            var result = dictionary.GetDictionaryValueType();

            // Assert
            result.Should().Be(expected);
        }
    }
}
