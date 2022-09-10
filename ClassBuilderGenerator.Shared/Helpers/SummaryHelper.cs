using Shared.Extensions;
using Shared.Models;

using System.Text;

namespace Shared.Helpers
{
    public static class SummaryHelper
    {
        public static StringBuilder AddClassSummary(this StringBuilder stringBuilder, bool configEnabled, ClassInformation classInformation)
        {
            if (!configEnabled)
                return stringBuilder;

            stringBuilder
                .AppendTab()
                .AppendLine("/// <summary>")
                .AppendTab()
                .Append("/// Builder for the class <see cref=\"")
                .Append(classInformation.Name)
                .Append("\">")
                .Append(classInformation.Name)
                .AppendLine("</see>")
                .AppendTab()
                .AppendLine("/// </summary>");

            return stringBuilder;
        }

        public static StringBuilder AddBuilderConstructorSummary(this StringBuilder stringBuilder, bool configEnabled, ClassInformation classInformation)
        {
            if (!configEnabled)
                return stringBuilder;

            stringBuilder
                .AppendTab(2)
                .AppendLine("/// <summary>")
                .AppendTab(2)
                .Append("/// Create a new instance for the <see cref=\"")
                .Append(classInformation.BuilderName)
                .Append("\">")
                .Append(classInformation.BuilderName)
                .AppendLine("</see>")
                .AppendTab(2)
                .AppendLine("/// </summary>");

            return stringBuilder;
        }

        public static StringBuilder AddResetSummary(this StringBuilder stringBuilder, bool configEnabled, ClassInformation classInformation)
        {
            if (!configEnabled)
                return stringBuilder;

            stringBuilder
                .AppendTab(2)
                .AppendLine("/// <summary>")
                .AppendTab(2)
                .AppendLine("/// Reset all properties' to the default value")
                .AppendTab(2)
                .AppendLine("/// </summary>")
                .AppendTab(2)
                .Append("/// <returns>Returns the <see cref=\"")
                .Append(classInformation.BuilderName)
                .Append("\">")
                .Append(classInformation.BuilderName)
                .AppendLine("</see> with the properties reseted</returns>");

            return stringBuilder;
        }

        public static StringBuilder AddWithSummary(this StringBuilder stringBuilder, bool configEnabled, ClassInformation classInformation, PropertyInformation propertyInformation)
        {
            if (!configEnabled)
                return stringBuilder;

            var propType = propertyInformation.Type;

            if (propertyInformation.CollectionType.IsCollection())
            {
                var collectionType = propType.GetEnumerableKeyType();

                stringBuilder
                    .AppendTab(2).AppendLine("/// <summary>")
                    .AppendTab(2).AppendFormat("/// Set a value of type <see cref=\"{0}\" /> of <see cref=\"{1}\" /> for the property <paramref name=\"{2}\">{2}</paramref>", propType.GetEnumerableType() + "{T}", collectionType, propertyInformation.OriginalNameInCamelCase).AppendLine()
                    .AppendTab(2).AppendLine("/// </summary>")
                    .AppendTab(2).AppendFormat("/// <param name=\"{0}\">A value of type {1} of {2} will the defined for the property</param>", propertyInformation.OriginalNameInCamelCase, propType.GetEnumerableType(), collectionType).AppendLine()
                    .AppendTab(2).AppendFormat("/// <returns>Returns the <see cref=\"{0}\" /> with the property <paramref name=\"{1}\">{1}</paramref> defined</returns>", classInformation.BuilderName, propertyInformation.OriginalNameInCamelCase).AppendLine();

                return stringBuilder;
            }

            if (propertyInformation.CollectionType.IsKeyValue())
            {
                var dictionaryKey = propType.GetDictionaryKeyType();
                var dictionaryValue = propType.GetDictionaryValueType();

                stringBuilder
                    .AppendTab(2).AppendLine("/// <summary>")
                    .AppendTab(2).AppendFormat("/// Set a value of type <see cref=\"{0}\" /> of <see cref=\"{1}\" /> and <see cref=\"{2}\" /> for the property <paramref name=\"{3}\">{3}</paramref>", propType.GetEnumerableType() + "{T,T}", dictionaryKey, dictionaryValue, propertyInformation.OriginalNameInCamelCase).AppendLine()
                    .AppendTab(2).AppendLine("/// </summary>")
                    .AppendTab(2).AppendFormat("/// <param name=\"{0}\">A value of type {1} of {2} will the defined for the property</param>", propertyInformation.OriginalNameInCamelCase, propType.GetEnumerableType(), dictionaryKey).AppendLine()
                    .AppendTab(2).AppendFormat("/// <returns>Returns the <see cref=\"{0}\" /> with the property <paramref name=\"{1}\">{1}</paramref> defined</returns>", classInformation.BuilderName, propertyInformation.OriginalNameInCamelCase).AppendLine();

                return stringBuilder;
            }

            stringBuilder
                .AppendTab(2).AppendLine("/// <summary>")
                .AppendTab(2).AppendFormat("/// Set a value of type <see cref=\"{0}\" /> for the property <paramref name=\"{1}\">{1}</paramref>", propertyInformation.Type, propertyInformation.OriginalNameInCamelCase).AppendLine()
                .AppendTab(2).AppendLine("/// </summary>")
                .AppendTab(2).AppendFormat("/// <param name=\"{0}\">A value of type {1} will the defined for the property</param>", propertyInformation.OriginalNameInCamelCase, propertyInformation.Type).AppendLine()
                .AppendTab(2).AppendFormat("/// <returns>Returns the <see cref=\"{0}\" /> with the property <paramref name=\"{1}\">{1}</paramref> defined</returns>", classInformation.BuilderName, propertyInformation.OriginalNameInCamelCase).AppendLine();

            return stringBuilder;
        }

        public static StringBuilder AddWithCollectionItemSummary(this StringBuilder stringBuilder, bool configEnabled, ClassInformation classInformation, PropertyInformation propertyInformation, string listObjectType)
        {
            if (!configEnabled)
                return stringBuilder;

            stringBuilder
                .AppendTab(2).AppendLine("/// <summary>")
                .AppendTab(2).AppendFormat("/// An item of type <see cref=\"{0}\"/> will be added to the collection {1}", listObjectType, propertyInformation.OriginalName.ToTitleCase()).AppendLine()
                .AppendTab(2).AppendLine("/// </summary>")
                .AppendTab(2).AppendFormat("/// <param name=\"item\">A value of type {0} will the added to the collection</param>", listObjectType).AppendLine()
                .AppendTab(2).Append("/// <returns>Returns the <see cref=\"").Append(classInformation.BuilderName).Append("\">").Append(classInformation.BuilderName).Append("</see> with the collection ").Append(propertyInformation.OriginalName.ToTitleCase()).AppendLine(" with one more item</returns>");

            return stringBuilder;
        }

        public static StringBuilder AddBuildSummary(this StringBuilder stringBuilder, bool configEnabled, ClassInformation classInformation)
        {
            if (!configEnabled)
                return stringBuilder;

            stringBuilder
                .AppendTab(2)
                .AppendLine("/// <summary>")
                .AppendTab(2)
                .Append("/// Build a class of type <see cref=\"")
                .Append(classInformation.Name)
                .Append("\">")
                .Append(classInformation.Name)
                .AppendLine("</see> with all the defined values")
                .AppendTab(2)
                .AppendLine("/// </summary>")
                .AppendTab(2)
                .Append("/// <returns>Returns a <see cref=\"")
                .Append(classInformation.Name)
                .Append("\">")
                .Append(classInformation.Name)
                .AppendLine("</see> class</returns>");

            return stringBuilder;
        }
    }
}
