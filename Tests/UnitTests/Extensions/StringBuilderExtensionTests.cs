using ClassBuilderGenerator.Extensions;

using FluentAssertions;

using System.Text;

using Xunit;

namespace UnitTests.Extensions
{
    public class StringBuilderExtensionTests
    {
        [Theory(DisplayName = "Should append the text when property is true")]
        [InlineData(true, "ABC", "123", "ABC123")]
        [InlineData(false, "ABC", "123", "ABC")]
        public void AppendWhenTrueTests(bool condition, string initialValue, string value, string expected)
        {
            // Arrange
            var builder = new StringBuilder();

            builder.Append(initialValue);

            // Act
            builder.AppendWhenTrue(condition, value);

            // Assert
            builder.ToString().Should().Be(expected);
        }

        [Theory(DisplayName = "Should append the correct text when property is true or false")]
        [InlineData(true, "ABC", "123", "456", "ABC123")]
        [InlineData(false, "ABC", "123", "456", "ABC456")]
        public void AppendWhenTests(bool condition, string initialValue, string valueWhenTrue, string valueWhenFalse, string expected)
        {
            // Arrange
            var builder = new StringBuilder();

            builder.Append(initialValue);

            // Act
            builder.AppendWhen(condition, valueWhenTrue, valueWhenFalse);

            // Assert
            builder.ToString().Should().Be(expected);
        }

        [Theory(DisplayName = "Should append the correct amount of tabs")]
        [InlineData(1, "ABC", "ABC\t")]
        [InlineData(3, "ABC", "ABC\t\t\t")]
        [InlineData(5, "ABC", "ABC\t\t\t\t\t")]
        public void AppendTabTests(int numOfTabs, string initialValue, string expected)
        {
            // Arrange
            var builder = new StringBuilder();

            builder.Append(initialValue);

            // Act
            builder.AppendTab(numOfTabs);

            // Assert
            builder.ToString().Should().Be(expected);
        }
    }
}
