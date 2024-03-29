﻿using ClassBuilderGenerator.Enums;

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ClassBuilderGenerator.Helpers
{
    public static class CollectionHelper
    {
        private static readonly Dictionary<string, CollectionType> collectionTypesPattern = new Dictionary<string, CollectionType>
        {
            { "^IList", CollectionType.IList },
            { "^List", CollectionType.List },
            { "^IEnumerable", CollectionType.IEnumerable },
            { "^Enumerable", CollectionType.Enumerable },
            { "^ICollection", CollectionType.ICollection },
            { "^Collection", CollectionType.Collection },
            { "^IDictionary", CollectionType.IDictionary },
            { "^Dictionary", CollectionType.Dictionary },
            { @"^([a-zA-Z])*(\[\])$", CollectionType.Array },
            { @"^([a-zA-Z])*(\[\]\[\])$", CollectionType.Matrix2Dim },
        };

        public static CollectionType GetCollectionType(string propertyType)
        {
            KeyValuePair<string, CollectionType> collectionType = collectionTypesPattern.FirstOrDefault(x => Regex.IsMatch(propertyType, x.Key));

            if (collectionType.Key is null)
                return CollectionType.None;

            return collectionType.Value;
        }
    }
}