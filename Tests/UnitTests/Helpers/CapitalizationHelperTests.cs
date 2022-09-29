using ClassBuilderGenerator.Helpers;

using FluentAssertions;

using Xunit;

namespace UnitTests.Helpers
{
    public class CapitalizationHelperTests
    {
        [Theory(DisplayName = "Should transform the word using the title case capitalization")]
        [InlineData("myProperty", "MyProperty")]
        [InlineData("MyProperty", "MyProperty")]
        [InlineData("My_Property", "My_Property")]
        [InlineData("", "")]
        [InlineData("a", "a")]
        public void ToTitleCaseTests(string input, string expected)
        {
            // Arrange

            // Act
            var result = input.ToTitleCase();

            // Assert
            result.Should().Be(expected);
        }

        [Theory(DisplayName = "Should transform the word using the camel case capitalization")]
        [InlineData("MyCurrentProperty", "myCurrentProperty")]
        [InlineData("Descending", "@descending")]
        public void ToCamelCaseTests(string input, string expected)
        {
            // Arrange

            // Act
            var result = input.ToCamelCase();

            // Assert
            result.Should().Be(expected);
        }
    }
}
