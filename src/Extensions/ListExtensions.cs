using System.Collections.Generic;

namespace ClassBuilderGenerator.Extensions
{
    public static class ListExtensions
    {
        public static void AddIfNotExists<T>(this List<T> source, T item)
        {
            if (!source.Contains(item))
            {
                source.Add(item);
            }
        }
    }
}
