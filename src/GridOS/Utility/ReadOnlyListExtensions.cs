using System;
using System.Collections.Generic;

namespace IngameScript
{
    public static class ReadOnlyListExtensions
    {
        public static int IndexOf<T>(this IReadOnlyList<T> self, T elementToFind, int notFoundValue = -1)
        {
            int i = 0;
            foreach (T element in self)
            {
                if (Equals(element, elementToFind))
                    return i;
                i++;
            }
            return notFoundValue;
        }

        public static int FindIndex<T>(this IReadOnlyList<T> self, Func<T, bool> predicate, int notFoundValue = -1)
        {
            if (predicate == null)
                throw new Exception("Predicate cannot be null.");

            int i = 0;
            foreach (T element in self)
            {
                if (predicate(element))
                    return i;
                i++;
            }
            return notFoundValue;
        }
    }
}
