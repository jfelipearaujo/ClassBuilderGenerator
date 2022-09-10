using Shared.Enums;

namespace Shared.Extensions
{
    public static class CollectionTypeExtensions
    {
        public static bool CanBeInstantiated(this CollectionType collectionType)
        {
            switch (collectionType)
            {
                case CollectionType.List:
                case CollectionType.Collection:
                case CollectionType.Dictionary:
                    return true;

                case CollectionType.IList:
                case CollectionType.Enumerable:
                case CollectionType.IEnumerable:
                case CollectionType.ICollection:
                case CollectionType.IDictionary:
                case CollectionType.None:
                default:
                    return false;
            };
        }

        public static bool IsValidCollection(this CollectionType collectionType)
        {
            return collectionType.IsEnumerable()
                || collectionType.IsCollection()
                || collectionType.IsKeyValue();
        }

        public static bool IsCollectionButNotKeyValue(this CollectionType collectionType)
        {
            return (collectionType.IsEnumerable() || collectionType.IsCollection())
                && !collectionType.IsKeyValue();
        }

        public static bool IsEnumerable(this CollectionType collectionType)
        {
            switch (collectionType)
            {
                case CollectionType.IEnumerable:
                case CollectionType.Enumerable:
                    return true;

                case CollectionType.ICollection:
                case CollectionType.Collection:
                case CollectionType.IList:
                case CollectionType.List:
                case CollectionType.IDictionary:
                case CollectionType.Dictionary:
                case CollectionType.None:
                default:
                    return false;
            };
        }

        public static bool IsCollection(this CollectionType collectionType)
        {
            switch (collectionType)
            {
                case CollectionType.IList:
                case CollectionType.List:
                case CollectionType.IEnumerable:
                case CollectionType.Enumerable:
                case CollectionType.ICollection:
                case CollectionType.Collection:
                    return true;

                case CollectionType.IDictionary:
                case CollectionType.Dictionary:
                case CollectionType.None:
                default:
                    return false;
            };
        }

        public static bool IsKeyValue(this CollectionType collectionType)
        {
            switch (collectionType)
            {
                case CollectionType.IList:
                case CollectionType.List:
                case CollectionType.IEnumerable:
                case CollectionType.Enumerable:
                case CollectionType.ICollection:
                case CollectionType.Collection:
                    return false;

                case CollectionType.IDictionary:
                case CollectionType.Dictionary:
                    return true;

                case CollectionType.None:
                default:
                    return false;
            };
        }
    }
}