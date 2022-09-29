using ClassBuilderGenerator.Helpers;

using FluentAssertions;

using Xunit;

namespace UnitTests.Helpers
{
    public class EnumerableHelperTests
    {
        [Theory(DisplayName = "Should return the correct collection type")]
        [InlineData("List<string>", "List")]
        [InlineData("IEnumerable<int>", "IEnumerable")]
        [InlineData("ICollection<MyClass>", "ICollection")]
        public void GetEnumerableTypeTests(string enumerable, string expected)
        {
            // Arrange

            // Act
            var result = enumerable.GetEnumerableType();

            // Assert
            result.Should().Be(expected);
        }

        [Theory(DisplayName = "Should return the correct collection key")]
        [InlineData("List<string>", "string")]
        [InlineData("IEnumerable<int>", "int")]
        [InlineData("ICollection<MyClass>", "MyClass")]
        public void GetEnumerableKeyTypeTests(string enumerable, string expected)
        {
            // Arrange

            // Act
            var result = enumerable.GetEnumerableKeyType();

            // Assert
            result.Should().Be(expected);
        }
    }
}
