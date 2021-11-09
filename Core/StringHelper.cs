﻿namespace ClassBuilderGenerator.Core
{
    public static class StringHelper
    {
        public static string ToCamelCase(this string str)
        {
            if(!string.IsNullOrEmpty(str) && str.Length > 1)
            {
                return AdjustIfIsReservedKeyword(char.ToLowerInvariant(str[0]) + str.Substring(1));
            }

            return AdjustIfIsReservedKeyword(str);
        }

        private static string AdjustIfIsReservedKeyword(string propName)
        {
            return BuilderConstants.ReservedKeywords.Contains(propName) ? $"@{propName}" : propName;
        }
    }
}