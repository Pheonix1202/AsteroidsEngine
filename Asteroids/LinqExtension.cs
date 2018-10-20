using System;
using System.Collections.Generic;

namespace Asteroids
{
    public static class LinqExtension
    {
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (T item in collection)
                action(item);
            return collection;
        }
    }
}
