using System.Text;

namespace ClassBuilderGenerator.Core
{
    public static class SummaryHelper
    {
        public static StringBuilder AddClassSummary(this StringBuilder stringBuilder, bool configEnabled, ClassInformation classInformation)
        {
            if(!configEnabled)
                return stringBuilder;

            stringBuilder
                .Append("\t")
                .AppendLine("/// <summary>")
                .Append("\t")
                .Append("/// Builder for the class <see cref=\"")
                .Append(classInformation.Name)
                .Append("\">")
                .Append(classInformation.Name)
                .AppendLine("</see>")
                .Append("\t")
                .AppendLine("/// <summary>");

            return stringBuilder;
        }

        public static StringBuilder AddBuilderConstructorSummary(this StringBuilder stringBuilder, bool configEnabled, ClassInformation classInformation)
        {
            if(!configEnabled)
                return stringBuilder;

            stringBuilder
                .Append("\t")
                .Append("\t")
                .AppendLine("/// <summary>")
                .Append("\t")
                .Append("\t")
                .Append("/// Create a new instance for the <see cref=\"")
                .Append(classInformation.BuilderName)
                .Append("\">")
                .Append(classInformation.BuilderName)
                .AppendLine("</see>")
                .Append("\t")
                .Append("\t")
                .AppendLine("/// <summary>");

            return stringBuilder;
        }

        public static StringBuilder AddResetSummary(this StringBuilder stringBuilder, bool configEnabled, ClassInformation classInformation)
        {
            if(!configEnabled)
                return stringBuilder;

            stringBuilder
                .Append("\t")
                .Append("\t")
                .AppendLine("/// <summary>")
                .Append("\t")
                .Append("\t")
                .AppendLine("/// Reset all properties' to the default value")
                .Append("\t")
                .Append("\t")
                .AppendLine("/// <summary>")
                .Append("\t")
                .Append("\t")
                .Append("/// <returns>Returns the <see cref=\"")
                .Append(classInformation.BuilderName)
                .Append("\">")
                .Append(classInformation.BuilderName)
                .AppendLine("</see> with the properties reseted</returns>");

            return stringBuilder;
        }

        public static StringBuilder AddWithSummary(this StringBuilder stringBuilder, bool configEnabled, ClassInformation classInformation, PropertyInformation propertyInformation)
        {
            if(!configEnabled)
                return stringBuilder;

            stringBuilder
                .Append("\t")
                .Append("\t")
                .AppendLine("/// <summary>")
                .Append("\t")
                .Append("\t")
                .Append("/// Set a value of type <typeparamref name=\"")
                .Append(propertyInformation.Type)
                .Append("\"/> for the property <paramref name=\"")
                .Append(propertyInformation.OriginalNameInCamelCase)
                .Append("\">")
                .Append(propertyInformation.OriginalNameInCamelCase)
                .AppendLine("</paramref>")
                .Append("\t")
                .Append("\t")
                .AppendLine("/// <summary>")
                .Append("\t")
                .Append("\t")
                .Append("/// <param name=\"")
                .Append(propertyInformation.OriginalNameInCamelCase)
                .Append("\">A value of type <typeparamref name=\"")
                .Append(propertyInformation.Type)
                .AppendLine("\"/> will the defined for the property</param>")
                .Append("\t")
                .Append("\t")
                .Append("/// <returns>Returns the <see cref=\"")
                .Append(classInformation.BuilderName)
                .Append("\">")
                .Append(classInformation.BuilderName)
                .Append("</see> with the property <paramref name=\"")
                .Append(propertyInformation.OriginalNameInCamelCase)
                .Append("\">")
                .Append(propertyInformation.OriginalNameInCamelCase)
                .AppendLine("</paramref> defined</returns>");

            return stringBuilder;
        }

        public static StringBuilder AddWithCollectionItemSummary(this StringBuilder stringBuilder, bool configEnabled, ClassInformation classInformation, PropertyInformation propertyInformation, string listObjectType)
        {
            if(!configEnabled)
                return stringBuilder;

            stringBuilder
                .Append("\t")
                .Append("\t")
                .AppendLine("/// <summary>")
                .Append("\t")
                .Append("\t")
                .Append("/// An item of type <typeparamref name=\"")
                .Append(listObjectType)
                .Append("\"/> will be added to the collection <c>")
                .Append(propertyInformation.OriginalName.ToTitleCase())
                .AppendLine("</c>")
                .Append("\t")
                .Append("\t")
                .AppendLine("/// <summary>")
                .Append("\t")
                .Append("\t")
                .Append("/// <param name=\"item\">A value of type <typeparamref name=\"")
                .Append(listObjectType)
                .AppendLine("\"/> will the added to the collection</param>")
                .Append("\t")
                .Append("\t")
                .Append("/// <returns>Returns the <see cref=\"")
                .Append(classInformation.BuilderName)
                .Append("\">")
                .Append(classInformation.BuilderName)
                .Append("</see> with the collection <c>")
                .Append(propertyInformation.OriginalName.ToTitleCase())
                .AppendLine("</c> with one more item</returns>");

            return stringBuilder;
        }

        public static StringBuilder AddBuildSummary(this StringBuilder stringBuilder, bool configEnabled, ClassInformation classInformation)
        {
            if(!configEnabled)
                return stringBuilder;

            stringBuilder
                .Append("\t")
                .Append("\t")
                .AppendLine("/// <summary>")
                .Append("\t")
                .Append("\t")
                .Append("/// Build a class of type <see cref=\"")
                .Append(classInformation.Name)
                .Append("\">")
                .Append(classInformation.Name)
                .AppendLine("</see> with all the defined values")
                .Append("\t")
                .Append("\t")
                .AppendLine("/// <summary>")
                .Append("\t")
                .Append("\t")
                .Append("/// <returns>Returns a <see cref=\"")
                .Append(classInformation.Name)
                .Append("\">")
                .Append(classInformation.Name)
                .AppendLine("</see> class</returns>");

            return stringBuilder;
        }
    }
}
