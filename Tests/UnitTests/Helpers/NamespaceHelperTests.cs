using FluentAssertions;

using Shared.Helpers;
using Shared.Models;

using System.Collections.Generic;

using Xunit;

namespace UnitTests.Helpers
{
    public class NamespaceHelperTests
    {
        public static IEnumerable<object[]> TestData()
        {
            yield return new object[] { "MyClass", "MyClass", default(ClassInformation) };
            yield return new object[] { "MyProject.MySubFolder.MyClass", "MyClass", default(ClassInformation) };
            yield return new object[] { "System.Collections.Generic.List<MyClass>", "List<MyClass>", default(ClassInformation) };
            yield return new object[] { "System.Collections.Generic.List<MyProject.MySubFolder.MyClass>", "List<MyClass>", default(ClassInformation) };
            yield return new object[] { "System.Collections.Generic.Dictionary<Guid, MyProject.MySubFolder.MyClass>", "Dictionary<Guid, MyClass>", default(ClassInformation) };
            yield return new object[] { "System.int", "int", default(ClassInformation) };
            yield return new object[] { "System.DateTime", "DateTime", default(ClassInformation) };
            yield return new object[] { "MyProject.MySubFolder.CustomObject", "CustomObject", default(ClassInformation) };
            yield return new object[] { "System.Collections.Generic.List<int>", "List<int>", default(ClassInformation) };
            yield return new object[] { "System.Collections.Generic.List<System.Datetime>", "List<Datetime>", default(ClassInformation) };
            yield return new object[] { "System.Collections.Generic.List<MyProject.MySubFolder.CustomObject>", "List<CustomObject>", default(ClassInformation) };
            yield return new object[] { "System.Collections.Generic.List<System.Collections.Generic.List<MyProject.MySubFolder.CustomObject>>", "List<List<CustomObject>>", default(ClassInformation) };
            yield return new object[] { "System.Collections.Generic.List<System.Collections.Generic.List<int>>", "List<List<int>>", default(ClassInformation) };
            yield return new object[] { "System.Collections.Generic.List<System.Collections.Generic.List<System.int>>", "List<List<int>>", default(ClassInformation) };
            yield return new object[] { "System.Collections.Generic.Dictionary<int, string>", "Dictionary<int, string>", default(ClassInformation) };
            yield return new object[] { "System.Collections.Generic.Dictionary<System.int, System.string>", "Dictionary<int, string>", default(ClassInformation) };
            yield return new object[] { "System.Collections.Generic.Dictionary<System.int, MyProject.MySubFolder.CustomObject>", "Dictionary<int, CustomObject>", default(ClassInformation) };
            yield return new object[] { "System.Collections.Generic.Dictionary<System.int, System.Collections.Generic.List<int>", "Dictionary<int, List<int>>", default(ClassInformation) };
            yield return new object[] { "System.Collections.Generic.Dictionary<System.int, System.Collections.Generic.List<MyProject.MySubFolder.CustomObject>", "Dictionary<int, List<CustomObject>>", default(ClassInformation) };

            yield return new object[] { "MyProject.MySubFolder.MyClass", "MyClass", new ClassInformation() };
            yield return new object[] { "MyProject.MySubFolder.CustomObject", "CustomObject", new ClassInformation() };
            yield return new object[] { "System.Collections.Generic.List<MyProject.MySubFolder.MyClass>", "List<MyClass>", new ClassInformation() };
            yield return new object[] { "System.Collections.Generic.Dictionary<Guid, MyProject.MySubFolder.MyClass>", "Dictionary<Guid, MyClass>", new ClassInformation() };
            yield return new object[] { "System.Collections.Generic.List<System.Collections.Generic.List<MyProject.MySubFolder.CustomObject>>", "List<List<CustomObject>>", new ClassInformation() };
        }

        [Theory(DisplayName = "Should remove correctly the namespace from the property")]
        [MemberData(nameof(TestData))]
        public void RemoveNamespaceTests(string input, string expected, ClassInformation classInformation)
        {
            // Arrange

            // Act
            var result = input.RemoveNamespace(classInformation);

            // Assert
            result.Should().Be(expected);

            if (classInformation != null)
            {
                classInformation.Usings.Should().Contain("MyProject.MySubFolder");
            }
        }
    }
}
