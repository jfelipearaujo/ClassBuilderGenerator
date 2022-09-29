using ClassBuilderGenerator.Enums;
using ClassBuilderGenerator.Helpers;

using FluentAssertions;

using Xunit;

namespace UnitTests.Helpers
{
    public class CollectionHelperTests
    {
        [Theory(DisplayName = "Should return the correct collection type")]
        [InlineData("IList<int>", CollectionType.IList)]
        [InlineData("List<int>", CollectionType.List)]
        [InlineData("IEnumerable<int>", CollectionType.IEnumerable)]
        [InlineData("Enumerable<int>", CollectionType.Enumerable)]
        [InlineData("ICollection<int>", CollectionType.ICollection)]
        [InlineData("Collection<int>", CollectionType.Collection)]
        [InlineData("IDictionary<int, string>", CollectionType.IDictionary)]
        [InlineData("Dictionary<int, string>", CollectionType.Dictionary)]
        [InlineData("int", CollectionType.None)]
        public void GetCollectionTypeTests(string collectionType, CollectionType expected)
        {
            // Arrange

            // Act
            var result = CollectionHelper.GetCollectionType(collectionType);

            // Assert
            result.Should().Be(expected);
        }
    }
}
