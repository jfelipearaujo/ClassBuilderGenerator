using FluentAssertions;

using Shared.Helpers;
using Shared.Models;

using System.Text;

using Xunit;

namespace UnitTests.Helpers
{
    public class SummaryHelperTests
    {
        public static TheoryData<bool, ClassInformation, string> AddClassSummaryTestData = new TheoryData<bool, ClassInformation, string>
        {
            { false, default, "" },
            {
                true,
                new ClassInformation{ Name = "MyClass" },
                "\t/// <summary>\r\n" +
                "\t/// Builder for the class <see cref=\"MyClass\">MyClass</see>\r\n" +
                "\t/// </summary>\r\n"
            },
        };

        public static TheoryData<bool, ClassInformation, string> AddBuilderConstructorSummaryTestData = new TheoryData<bool, ClassInformation, string>
        {
            { false, default, "" },
            {
                true,
                new ClassInformation{ Name = "MyClass" },
                "\t\t/// <summary>\r\n" +
                "\t\t/// Create a new instance for the <see cref=\"MyClassBuilder\">MyClassBuilder</see>\r\n" +
                "\t\t/// </summary>\r\n"
            },
        };

        public static TheoryData<bool, ClassInformation, string> AddResetSummaryTestData = new TheoryData<bool, ClassInformation, string>
        {
            { false, default, "" },
            {
                true,
                new ClassInformation{ Name = "MyClass" },
                "\t\t/// <summary>\r\n" +
                "\t\t/// Reset all properties' to the default value\r\n" +
                "\t\t/// </summary>\r\n" +
                "\t\t/// <returns>Returns the <see cref=\"MyClassBuilder\">MyClassBuilder</see> with the properties reseted</returns>\r\n"
            },
        };

        public static TheoryData<bool, ClassInformation, PropertyInformation, string> AddWithSummaryTestData = new TheoryData<bool, ClassInformation, PropertyInformation, string>
        {
            { false, default, default, "" },
            {
                true,
                new ClassInformation{ Name = "MyClass" },
                new PropertyInformation { Type = "int", OriginalName = "FirstName" },
                "\t\t/// <summary>\r\n" +
                "\t\t/// Set a value of type <see cref=\"int\" /> for the property <paramref name=\"firstName\">firstName</paramref>\r\n" +
                "\t\t/// </summary>\r\n" +
                "\t\t/// <param name=\"firstName\">A value of type int will the defined for the property</param>\r\n" +
                "\t\t/// <returns>Returns the <see cref=\"MyClassBuilder\" /> with the property <paramref name=\"firstName\">firstName</paramref> defined</returns>\r\n"
            },
            {
                true,
                new ClassInformation{ Name = "MyClass" },
                new PropertyInformation { Type = "List<string>", OriginalName = "PhoneNumbers" },
                "\t\t/// <summary>\r\n" +
                "\t\t/// Set a value of type <see cref=\"List{T}\" /> of <see cref=\"string\" /> for the property <paramref name=\"phoneNumbers\">phoneNumbers</paramref>\r\n" +
                "\t\t/// </summary>\r\n" +
                "\t\t/// <param name=\"phoneNumbers\">A value of type List of string will the defined for the property</param>\r\n" +
                "\t\t/// <returns>Returns the <see cref=\"MyClassBuilder\" /> with the property <paramref name=\"phoneNumbers\">phoneNumbers</paramref> defined</returns>\r\n"
            },
            {
                true,
                new ClassInformation{ Name = "MyClass" },
                new PropertyInformation { Type = "Dictionary<Guid, string>", OriginalName = "Tasks" },
                "\t\t/// <summary>\r\n" +
                "\t\t/// Set a value of type <see cref=\"Dictionary{T,T}\" /> of <see cref=\"Guid\" /> and <see cref=\"string\" /> for the property <paramref name=\"tasks\">tasks</paramref>\r\n" +
                "\t\t/// </summary>\r\n" +
                "\t\t/// <param name=\"tasks\">A value of type Dictionary of Guid will the defined for the property</param>\r\n" +
                "\t\t/// <returns>Returns the <see cref=\"MyClassBuilder\" /> with the property <paramref name=\"tasks\">tasks</paramref> defined</returns>\r\n"
            },
        };

        public static TheoryData<bool, ClassInformation, PropertyInformation, string> AddWithCollectionItemSummaryTestData = new TheoryData<bool, ClassInformation, PropertyInformation, string>
        {
            { false, default, default, "" },
            {
                true,
                new ClassInformation{ Name = "MyClass" },
                new PropertyInformation { Type = "List<int>", OriginalName = "Sizes" },
                "\t\t/// <summary>\r\n" +
                "\t\t/// An item of type <see cref=\"int\"/> will be added to the collection Sizes\r\n" +
                "\t\t/// </summary>\r\n" +
                "\t\t/// <param name=\"item\">A value of type int will the added to the collection</param>\r\n" +
                "\t\t/// <returns>Returns the <see cref=\"MyClassBuilder\" /> with the collection Sizes with one more item</returns>\r\n"
            },
        };

        public static TheoryData<bool, ClassInformation, string> AddBuildSummaryTestData = new TheoryData<bool, ClassInformation, string>
        {
            { false, default, "" },
            {
                true,
                new ClassInformation{ Name = "MyClass" },
                "\t\t/// <summary>\r\n" +
                "\t\t/// Build a class of type <see cref=\"MyClass\">MyClass</see> with all the defined values\r\n" +
                "\t\t/// </summary>\r\n" +
                "\t\t/// <returns>Returns a <see cref=\"MyClass\">MyClass</see> class</returns>\r\n"
            },
        };

        [Theory]
        [MemberData(nameof(AddClassSummaryTestData))]
        public void AddClassSummaryTests(bool configEnabled, ClassInformation classInformation, string expected)
        {
            // Arrange
            var builder = new StringBuilder();

            // Act
            builder.AddClassSummary(configEnabled, classInformation);

            // Assert
            var result = builder.ToString();

            result.Should().Be(expected);
        }

        [Theory]
        [MemberData(nameof(AddBuilderConstructorSummaryTestData))]
        public void AddBuilderConstructorSummaryTests(bool configEnabled, ClassInformation classInformation, string expected)
        {
            // Arrange
            var builder = new StringBuilder();

            // Act
            builder.AddBuilderConstructorSummary(configEnabled, classInformation);

            // Assert
            var result = builder.ToString();

            result.Should().Be(expected);
        }

        [Theory]
        [MemberData(nameof(AddResetSummaryTestData))]
        public void AddResetSummaryTests(bool configEnabled, ClassInformation classInformation, string expected)
        {
            // Arrange
            var builder = new StringBuilder();

            // Act
            builder.AddResetSummary(configEnabled, classInformation);

            // Assert
            var result = builder.ToString();

            result.Should().Be(expected);
        }

        [Theory]
        [MemberData(nameof(AddWithSummaryTestData))]
        public void AddWithSummaryTests(bool configEnabled, ClassInformation classInformation, PropertyInformation propertyInformation, string expected)
        {
            // Arrange
            var builder = new StringBuilder();

            // Act
            builder.AddWithSummary(configEnabled, classInformation, propertyInformation);

            // Assert
            var result = builder.ToString();

            result.Should().Be(expected);
        }

        [Theory]
        [MemberData(nameof(AddWithCollectionItemSummaryTestData))]
        public void AddWithCollectionItemSummaryTests(bool configEnabled, ClassInformation classInformation, PropertyInformation propertyInformation, string expected)
        {
            // Arrange
            var builder = new StringBuilder();

            // Act
            builder.AddWithCollectionItemSummary(configEnabled, classInformation, propertyInformation);

            // Assert
            var result = builder.ToString();

            result.Should().Be(expected);
        }

        [Theory]
        [MemberData(nameof(AddBuildSummaryTestData))]
        public void AddBuildSummaryTests(bool configEnabled, ClassInformation classInformation, string expected)
        {
            // Arrange
            var builder = new StringBuilder();

            // Act
            builder.AddBuildSummary(configEnabled, classInformation);

            // Assert
            var result = builder.ToString();

            result.Should().Be(expected);
        }
    }
}
