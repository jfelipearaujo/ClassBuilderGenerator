using System.Linq;
using System.Text.RegularExpressions;

namespace ClassBuilderGenerator.Core
{
    public static class StringHelper
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
            else if (str.Contains("."))
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

        public static string ToTitleCase(this string str)
        {
            if (!string.IsNullOrEmpty(str) && str.Length > 1)
            {
                return char.ToUpperInvariant(str[0]) + str.Substring(1);
            }

            return str;
        }

        public static string ToCamelCase(this string str)
        {
            var x = str.Replace("_", "");

            if (x.Length == 0)
                return AdjustIfIsReservedKeyword(str);

            x = Regex.Replace(x, "([A-Z])([A-Z]+)($|[A-Z])",
                m => m.Groups[1].Value + m.Groups[2].Value.ToLower() + m.Groups[3].Value);

            return AdjustIfIsReservedKeyword(char.ToLowerInvariant(x[0]) + x.Substring(1));
        }

        private static string AdjustIfIsReservedKeyword(string propName)
        {
            return BuilderConstants.ReservedKeywords.Contains(propName) ? $"@{propName}" : propName;
        }

        public static bool RegexMatch(this string str, string pattern)
        {
            return Regex.IsMatch(str, pattern);
        }

        public static string GetDictionaryKeyType(this string str)
        {
            var dicBase = str.Split(',').First();
            var key = dicBase.Substring(dicBase.IndexOf("<") + 1);
            return RemoveNamespace(key);
        }

        public static string GetDictionaryValueType(this string str)
        {
            var dicBase = str.Split(',').ElementAt(1).TrimStart();
            var key = dicBase.Substring(0, dicBase.LastIndexOf(">"));
            return RemoveNamespace(key);
        }

        public static string GetIEnumerableKeyType(this string str)
        {
            var key = str.Substring(str.IndexOf("<") + 1);
            return RemoveNamespace(key.Substring(0, key.LastIndexOf(">")));
        }
    }
}
