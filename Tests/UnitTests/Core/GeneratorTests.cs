using ClassBuilderGenerator.Core;
using ClassBuilderGenerator.Enums;
using ClassBuilderGenerator.Models;

using FluentAssertions;

using System.Collections.Generic;
using System.IO;
using System.Text;

using Xunit;

namespace UnitTests.Core
{
    public class GeneratorTests
    {
        [Fact(DisplayName = "Should generate the builder class correctly")]
        public void GenerateBuilderTests()
        {
            // Arrange
            var classInformation = new ClassInformation
            {
                Name = "User",
                Namespace = "ConsoleApp1",
                IsPublicAccessible = true,
                Properties = new List<PropertyInformation>
                {
                    new PropertyInformation
                    {
                        OriginalName = "Id",
                        Type = "Guid"
                    },
                    new PropertyInformation
                    {
                        OriginalName = "FirstName",
                        Type = "string"
                    },
                    new PropertyInformation
                    {
                        OriginalName = "LastName",
                        Type = "string"
                    },
                    new PropertyInformation
                    {
                        OriginalName = "Email",
                        Type = "string"
                    },
                    new PropertyInformation
                    {
                        OriginalName = "Phone",
                        Type = "string"
                    },
                    new PropertyInformation
                    {
                        OriginalName = "IsAlive",
                        Type = "bool"
                    },
                    new PropertyInformation
                    {
                        OriginalName = "Age",
                        Type = "int"
                    },
                    new PropertyInformation
                    {
                        OriginalName = "Addresses",
                        Type = "List<string>"
                    },
                    new PropertyInformation
                    {
                        OriginalName = "OtherAddresses",
                        Type = "IList<string>"
                    },
                    new PropertyInformation
                    {
                        OriginalName = "Ages",
                        Type = "IEnumerable<int>"
                    },
                    new PropertyInformation
                    {
                        OriginalName = "Phones",
                        Type = "Dictionary<string, int>"
                    },
                    new PropertyInformation
                    {
                        OriginalName = "OtherPhones",
                        Type = "IDictionary<string, int>"
                    },
                    new PropertyInformation
                    {
                        OriginalName = "Collection",
                        Type = "Collection<int>"
                    },
                    new PropertyInformation
                    {
                        OriginalName = "OtherCollection",
                        Type = "ICollection<int>"
                    },
                    new PropertyInformation
                    {
                        OriginalName = "IntArray",
                        Type = "int[]"
                    },
                    new PropertyInformation
                    {
                        OriginalName = "IntMatrix",
                        Type = "int[][]"
                    },
                    new PropertyInformation
                    {
                        OriginalName = "BoolArray",
                        Type = "bool[]"
                    },
                    new PropertyInformation
                    {
                        OriginalName = "BoolMatrix",
                        Type = "bool[][]"
                    },
                }
            };

            var builder = new StringBuilder();

            var options = new GeneratorOptions
            {
                GenerateListWithItemMethod = true,
                MethodWithGenerator = MethodWithGenerator.GenerateAllProps,
                GenerateSummaryInformation = true,
                GenerateWithMethodForCollections = true,
                AddUnderscorePrefixToTheFields = true,
            };

            // Act
            Generator.GenerateBuilder(classInformation, builder, options);

            // Assert
            var expected = File.ReadAllText(Path.Combine("Core", "BuilderResult.txt"), Encoding.UTF8);

            var result = builder.ToString();

            result.Should().Be(expected);
        }
    }
}
