namespace Shared.Helpers
{
    public static class EnumerableHelper
    {
        public static string GetEnumerableType(this string str)
        {
            return str.Substring(0, str.LastIndexOf("<")).RemoveNamespace();
        }

        public static string GetEnumerableKeyType(this string str)
        {
            var key = str.Substring(str.IndexOf("<") + 1);

            return key.Substring(0, key.LastIndexOf(">")).RemoveNamespace();
        }
    }
}