using Shared.Enums;
using Shared.Helpers;

namespace Shared.Models
{
    public class PropertyInformation
    {
        public string Type { get; set; }
        public string OriginalName { get; set; }


        private string _originalNameInCamelCase = null;
        public string OriginalNameInCamelCase
        {
            get
            {
                if (_originalNameInCamelCase is null)
                {
                    _originalNameInCamelCase = OriginalName.ToCamelCase();
                }

                return _originalNameInCamelCase;
            }
        }

        private CollectionType? _collectionType = null;
        public CollectionType CollectionType
        {
            get
            {
                if (_collectionType is null)
                {
                    _collectionType = CollectionHelper.GetCollectionType(Type);
                }

                return _collectionType.Value;
            }
        }
    }
}