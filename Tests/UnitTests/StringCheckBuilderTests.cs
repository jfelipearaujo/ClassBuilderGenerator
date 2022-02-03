using ClassBuilderGenerator.Core;

using FluentAssertions;

using Xunit;

namespace UnitTests
{
    public class StringCheckBuilderTests
    {
        private readonly StringCheckBuilder sut;

        public StringCheckBuilderTests()
        {
            sut = new StringCheckBuilder();
        }

        [Theory]
        [InlineData("List")]
        [InlineData("IEnumerable")]
        [InlineData("ICollection")]
        [InlineData("Collection")]
        public void CheckForOrCondition_CheckForAllConditions(string input)
        {
            // Arrange

            // Act
            var result = sut.IsList(input)
                .IsIEnumerable(input)
                .IsICollection(input)
                .IsCollection(input)
                .CheckForOrCondition();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void CheckForOrCondition_CheckIfIsList()
        {
            // Arrange
            const string input = "List";

            // Act
            var result = sut.IsList(input)
                .CheckForOrCondition();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void CheckForOrCondition_CheckIfIsIEnumerable()
        {
            // Arrange
            const string input = "IEnumerable";

            // Act
            var result = sut.IsIEnumerable(input)
                .CheckForOrCondition();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void CheckForOrCondition_CheckIfIsCollection()
        {
            // Arrange
            const string input = "Collection";

            // Act
            var result = sut.IsCollection(input)
                .CheckForOrCondition();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void CheckForOrCondition_CheckIfIsICollection()
        {
            // Arrange
            const string input = "ICollection";

            // Act
            var result = sut.IsICollection(input)
                .CheckForOrCondition();

            // Assert
            result.Should().BeTrue();
        }
    }
}
