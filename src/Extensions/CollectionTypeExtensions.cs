using ClassBuilderGenerator.Enums;

namespace ClassBuilderGenerator.Extensions
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
            return collectionType.IsEnumerableType()
                || collectionType.IsCollection()
                || collectionType.IsDictionaryType();
        }

        public static bool IsValidArrayOrMatrix(this CollectionType collectionType)
        {
            return collectionType.IsArrayType()
                || collectionType.IsMatrix2DimType();
        }

        public static bool IsCollectionButNotKeyValue(this CollectionType collectionType)
        {
            return (collectionType.IsEnumerableType() || collectionType.IsCollection())
                && !collectionType.IsDictionaryType();
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

        public static bool IsArrayType(this CollectionType collectionType)
        {
            switch (collectionType)
            {
                case CollectionType.Array:
                    return true;

                case CollectionType.Matrix2Dim:
                case CollectionType.IList:
                case CollectionType.List:
                case CollectionType.IEnumerable:
                case CollectionType.Enumerable:
                case CollectionType.ICollection:
                case CollectionType.Collection:
                case CollectionType.IDictionary:
                case CollectionType.Dictionary:
                case CollectionType.None:
                default:
                    return false;
            };
        }

        public static bool IsMatrix2DimType(this CollectionType collectionType)
        {
            switch (collectionType)
            {
                case CollectionType.Matrix2Dim:
                    return true;

                case CollectionType.Array:
                case CollectionType.IList:
                case CollectionType.List:
                case CollectionType.IEnumerable:
                case CollectionType.Enumerable:
                case CollectionType.ICollection:
                case CollectionType.Collection:
                case CollectionType.IDictionary:
                case CollectionType.Dictionary:
                case CollectionType.None:
                default:
                    return false;
            };
        }

        public static bool IsListType(this CollectionType collectionType)
        {
            switch (collectionType)
            {
                case CollectionType.IList:
                case CollectionType.List:
                    return true;

                case CollectionType.Array:
                case CollectionType.Matrix2Dim:
                case CollectionType.IEnumerable:
                case CollectionType.Enumerable:
                case CollectionType.ICollection:
                case CollectionType.Collection:
                case CollectionType.IDictionary:
                case CollectionType.Dictionary:
                case CollectionType.None:
                default:
                    return false;
            };
        }

        public static bool IsEnumerableType(this CollectionType collectionType)
        {
            switch (collectionType)
            {
                case CollectionType.IEnumerable:
                case CollectionType.Enumerable:
                    return true;

                case CollectionType.Array:
                case CollectionType.Matrix2Dim:
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

        public static bool IsCollectionType(this CollectionType collectionType)
        {
            switch (collectionType)
            {
                case CollectionType.ICollection:
                case CollectionType.Collection:
                    return true;

                case CollectionType.Array:
                case CollectionType.Matrix2Dim:
                case CollectionType.IList:
                case CollectionType.List:
                case CollectionType.IEnumerable:
                case CollectionType.Enumerable:
                case CollectionType.IDictionary:
                case CollectionType.Dictionary:
                case CollectionType.None:
                default:
                    return false;
            };
        }

        public static bool IsDictionaryType(this CollectionType collectionType)
        {
            switch (collectionType)
            {
                case CollectionType.IDictionary:
                case CollectionType.Dictionary:
                    return true;

                case CollectionType.Array:
                case CollectionType.Matrix2Dim:
                case CollectionType.IList:
                case CollectionType.List:
                case CollectionType.IEnumerable:
                case CollectionType.Enumerable:
                case CollectionType.ICollection:
                case CollectionType.Collection:
                case CollectionType.None:
                default:
                    return false;
            };
        }
    }
}