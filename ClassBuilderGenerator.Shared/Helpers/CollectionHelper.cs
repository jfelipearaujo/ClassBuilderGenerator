using Shared.Enums;

using System.Collections.Generic;
using System.Linq;

namespace Shared.Helpers
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
        };

        public static CollectionType GetCollectionType(string propertyType)
        {
            KeyValuePair<string, CollectionType> collectionType = collectionTypesPattern.FirstOrDefault(x => propertyType.RegexMatch(x.Key));

            if (collectionType.Key is null)
                return CollectionType.None;

            return collectionType.Value;
        }
    }
}