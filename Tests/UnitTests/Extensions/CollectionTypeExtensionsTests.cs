using ClassBuilderGenerator.Enums;
using ClassBuilderGenerator.Extensions;

using FluentAssertions;

using Xunit;

namespace UnitTests.Extensions
{
    public class CollectionTypeExtensionsTests
    {
        [Theory(DisplayName = "Should return if the collection could be instantiated")]
        [InlineData(CollectionType.None, false)]
        [InlineData(CollectionType.IList, false)]
        [InlineData(CollectionType.List, true)]
        [InlineData(CollectionType.IEnumerable, false)]
        [InlineData(CollectionType.Enumerable, false)]
        [InlineData(CollectionType.ICollection, false)]
        [InlineData(CollectionType.Collection, true)]
        [InlineData(CollectionType.IDictionary, false)]
        [InlineData(CollectionType.Dictionary, true)]
        public void CanBeInstantiatedTests(CollectionType collectionType, bool expected)
        {
            // Arrange

            // Act
            var result = collectionType.CanBeInstantiated();

            // Assert
            result.Should().Be(expected);
        }

        [Theory(DisplayName = "Should return if the collection is any valid collection")]
        [InlineData(CollectionType.None, false)]
        [InlineData(CollectionType.IList, true)]
        [InlineData(CollectionType.List, true)]
        [InlineData(CollectionType.IEnumerable, true)]
        [InlineData(CollectionType.Enumerable, true)]
        [InlineData(CollectionType.ICollection, true)]
        [InlineData(CollectionType.Collection, true)]
        [InlineData(CollectionType.IDictionary, true)]
        [InlineData(CollectionType.Dictionary, true)]
        public void IsValidCollectionTests(CollectionType collectionType, bool expected)
        {
            // Arrange

            // Act
            var result = collectionType.IsValidCollection();

            // Assert
            result.Should().Be(expected);
        }

        [Theory(DisplayName = "Should return if the collection is not a key value collection")]
        [InlineData(CollectionType.None, false)]
        [InlineData(CollectionType.IList, true)]
        [InlineData(CollectionType.List, true)]
        [InlineData(CollectionType.IEnumerable, true)]
        [InlineData(CollectionType.Enumerable, true)]
        [InlineData(CollectionType.ICollection, true)]
        [InlineData(CollectionType.Collection, true)]
        [InlineData(CollectionType.IDictionary, false)]
        [InlineData(CollectionType.Dictionary, false)]
        public void IsCollectionButNotKeyValueTests(CollectionType collectionType, bool expected)
        {
            // Arrange

            // Act
            var result = collectionType.IsCollectionButNotKeyValue();

            // Assert
            result.Should().Be(expected);
        }

        [Theory(DisplayName = "Should return if the collection is an enumerable")]
        [InlineData(CollectionType.None, false)]
        [InlineData(CollectionType.IList, false)]
        [InlineData(CollectionType.List, false)]
        [InlineData(CollectionType.IEnumerable, true)]
        [InlineData(CollectionType.Enumerable, true)]
        [InlineData(CollectionType.ICollection, false)]
        [InlineData(CollectionType.Collection, false)]
        [InlineData(CollectionType.IDictionary, false)]
        [InlineData(CollectionType.Dictionary, false)]
        public void IsEnumerableTests(CollectionType collectionType, bool expected)
        {
            // Arrange

            // Act
            var result = collectionType.IsEnumerableType();

            // Assert
            result.Should().Be(expected);
        }

        [Theory(DisplayName = "Should return if the collection is a collection")]
        [InlineData(CollectionType.None, false)]
        [InlineData(CollectionType.IList, true)]
        [InlineData(CollectionType.List, true)]
        [InlineData(CollectionType.IEnumerable, true)]
        [InlineData(CollectionType.Enumerable, true)]
        [InlineData(CollectionType.ICollection, true)]
        [InlineData(CollectionType.Collection, true)]
        [InlineData(CollectionType.IDictionary, false)]
        [InlineData(CollectionType.Dictionary, false)]
        public void IsCollectionTests(CollectionType collectionType, bool expected)
        {
            // Arrange

            // Act
            var result = collectionType.IsCollection();

            // Assert
            result.Should().Be(expected);
        }

        [Theory(DisplayName = "Should return if the collection is a key value collection")]
        [InlineData(CollectionType.None, false)]
        [InlineData(CollectionType.IList, false)]
        [InlineData(CollectionType.List, false)]
        [InlineData(CollectionType.IEnumerable, false)]
        [InlineData(CollectionType.Enumerable, false)]
        [InlineData(CollectionType.ICollection, false)]
        [InlineData(CollectionType.Collection, false)]
        [InlineData(CollectionType.IDictionary, true)]
        [InlineData(CollectionType.Dictionary, true)]
        public void IsKeyValueTests(CollectionType collectionType, bool expected)
        {
            // Arrange

            // Act
            var result = collectionType.IsDictionaryType();

            // Assert
            result.Should().Be(expected);
        }
    }
}
