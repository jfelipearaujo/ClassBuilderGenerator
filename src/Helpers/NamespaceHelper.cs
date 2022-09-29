using ClassBuilderGenerator.Models;

namespace ClassBuilderGenerator.Helpers
{
    public static class NamespaceHelper
    {
        public static string RemoveNamespace(this string str, ClassInformation classInformation = null)
        {
            if (str.Contains("<"))
            {
                var collectionNamespace = str.Substring(0, str.IndexOf("<"));

                if (classInformation != null)
                {
                    var collectionUsing = collectionNamespace.Substring(0, collectionNamespace.LastIndexOf("."));

                    if (!classInformation.Usings.Contains(collectionUsing))
                    {
                        classInformation.Usings.Add(collectionUsing);
                    }
                }

                var collectionObject = str.Substring(str.IndexOf("<") + 1);

                if (collectionObject.Contains(">"))
                    collectionObject = collectionObject.Substring(0, collectionObject.LastIndexOf(">"));

                var collectionType = collectionNamespace.RemoveNamespace(classInformation);

                // Check if is a dictionary type
                if (collectionObject.Contains(","))
                {
                    var dicTypes = collectionObject.Split(',');

                    for (int i = 0; i < dicTypes.Length; i++)
                        dicTypes[i] = dicTypes[i].TrimStart().TrimEnd().Trim();

                    var dicKey = dicTypes[0].RemoveNamespace(classInformation);
                    var dicValue = dicTypes[1].RemoveNamespace(classInformation);

                    return $"{collectionType}<{dicKey}, {dicValue}>";
                }

                return $"{collectionType}<{collectionObject.RemoveNamespace(classInformation)}>";
            }

            if (str.Contains("."))
            {
                if (classInformation != null)
                {
                    var propUsing = str.Substring(0, str.LastIndexOf("."));

                    if (!classInformation.Usings.Contains(propUsing))
                    {
                        classInformation.Usings.Add(propUsing);
                    }
                }

                return str.Substring(str.LastIndexOf(".") + 1);
            }

            return str;
        }
    }
}
