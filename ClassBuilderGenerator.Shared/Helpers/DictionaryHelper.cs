using System.Linq;

namespace Shared.Helpers
{
    public static class DictionaryHelper
    {
        public static string GetDictionaryKeyType(this string str)
        {
            var dicBase = str.Split(',').First();
            var key = dicBase.Substring(dicBase.IndexOf("<") + 1);

            return key.RemoveNamespace();
        }

        public static string GetDictionaryValueType(this string str)
        {
            var dicBase = str.Split(',').ElementAt(1).TrimStart();
            var key = dicBase.Substring(0, dicBase.LastIndexOf(">"));

            return key.RemoveNamespace();
        }
    }
}
