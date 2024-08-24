namespace Shaderlens.Extensions
{
    public static class EnumerableExtensions
    {
        public static int IndexOf<T>(this IEnumerable<T> source, T item)
        {
            var index = 0;

            foreach (var item1 in source)
            {
                if (Equals(item1, item))
                {
                    return index;
                }

                index++;
            }

            return -1;
        }

        public static int IndexOf<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            var index = 0;

            foreach (var item in source)
            {
                if (predicate(item))
                {
                    return index;
                }

                index++;
            }

            return -1;
        }

        public static IEnumerable<T> Concat<T>(this IEnumerable<T> source, T item)
        {
            return source.Concat(new[] { item });
        }
    }
}
