namespace ClassBuilderGenerator.Helpers
{
    public static class ArrayHelper
    {
        public static string GetArrayType(this string str)
        {
            return str.Substring(0, str.IndexOf("[")).RemoveNamespace();
        }
    }
}
