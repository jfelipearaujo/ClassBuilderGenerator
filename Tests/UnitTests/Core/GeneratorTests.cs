using FluentAssertions;

using Shared.Core;
using Shared.Enums;
using Shared.Models;

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
                        OriginalName = "Ages",
                        Type = "IEnumerable<int>"
                    },
                    new PropertyInformation
                    {
                        OriginalName = "Phones",
                        Type = "Dictionary<string, int>"
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
            var expected = File.ReadAllText(Path.Combine("Core", "BuilderResult.txt"));

            var result = builder.ToString();

            result.Should().Be(expected);
        }
    }
}
