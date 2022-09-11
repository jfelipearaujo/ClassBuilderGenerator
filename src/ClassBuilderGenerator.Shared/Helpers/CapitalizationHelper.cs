using Shared.Constants;

using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Shared.Helpers
{
    public static class CapitalizationHelper
    {
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
            var words = str.Split(new[] { "_", " " }, StringSplitOptions.RemoveEmptyEntries);

            var leadWord = Regex.Replace(words[0], @"([A-Z])([A-Z]+|[a-z0-9]+)($|[A-Z]\w*)",
                m =>
                {
                    return m.Groups[1].Value.ToLower() + m.Groups[2].Value.ToLower() + m.Groups[3].Value;
                });

            var tailWords = words.Skip(1)
                .Select(word => char.ToUpper(word[0]) + word.Substring(1))
                .ToArray();

            var result = $"{leadWord}{string.Join(string.Empty, tailWords)}";

            return AdjustIfIsReservedKeyword(result);
        }

        private static string AdjustIfIsReservedKeyword(string propName)
        {
            return BuilderConstants.ReservedKeywords.Contains(propName) ? $"@{propName}" : propName;
        }
    }
}
