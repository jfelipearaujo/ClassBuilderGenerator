using FluentAssertions;

using Shared.Extensions;

using System.Text;

using Xunit;

namespace UnitTests
{
    public class StringBuilderExtensionTests
    {
        [Theory]
        [InlineData(true, "ABC", "123", "ABC123")]
        [InlineData(false, "ABC", "123", "ABC")]
        public void AppendWhenTrue(bool condition, string initialValue, string value, string expected)
        {
            // Arrange
            var builder = new StringBuilder();

            builder.Append(initialValue);

            // Act
            builder.AppendWhenTrue(condition, value);

            // Assert
            builder.ToString().Should().Be(expected);
        }

        [Theory]
        [InlineData(true, "ABC", "123", "456", "ABC123")]
        [InlineData(false, "ABC", "123", "456", "ABC456")]
        public void AppendWhen(bool condition, string initialValue, string valueWhenTrue, string valueWhenFalse, string expected)
        {
            // Arrange
            var builder = new StringBuilder();

            builder.Append(initialValue);

            // Act
            builder.AppendWhen(condition, valueWhenTrue, valueWhenFalse);

            // Assert
            builder.ToString().Should().Be(expected);
        }
    }
}
