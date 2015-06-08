using System;
using System.Collections.Generic;
using System.Linq;

namespace MyToolkit.Utilities
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Instead of OrderBy(...).ThenBy(...) this method calls ThenBy only if necessary
        /// </summary>
        public static IEnumerable<TSource> OrderByThenBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> orderBy, Func<TSource, TKey> thenBy)
        {
            var sorted = source
                .Select(s => new Tuple<TSource, TKey>(s, orderBy(s)))
                .OrderBy(s => s.Item2)
                .GroupBy(s => s.Item2);

            var result = new List<TSource>();
            foreach (var s in sorted)
            {
                if (s.Count() > 1)
                    result.AddRange(s.Select(p => p.Item1).OrderBy(thenBy));
                else
                    result.Add(s.First().Item1);
            }
            return result;
        }

        /// <summary>Checks whether the values of a given a property are equal. </summary>
        /// <typeparam name="TItem">The type of an item. </typeparam>
        /// <typeparam name="TProperty">The type of the property to compare. </typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="propertySelector">The property selector.</param>
        /// <returns>Returns true if all values are equal.</returns>
        public static bool AreValuesEqual<TItem, TProperty>(this IEnumerable<TItem> enumerable, Func<TItem, TProperty> propertySelector)
        {
            var first = true;
            TProperty value = default(TProperty);
            foreach (var item in enumerable)
            {
                var last = value;
                value = propertySelector(item);
                if (!first && !value.Equals(last))
                    return false;
                first = false;
            }
            return true;
        }

        /// <summary>
        /// Returns a list of distinct items by specifing a selector to determine distinctiveness. 
        /// </summary>
        /// <typeparam name="TItem">The type of an item. </typeparam>
        /// <typeparam name="TProperty">The type of the property to compare. </typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="propertySelector">The property selector.</param>
        /// <returns>Returns the list of distinct items. </returns>
        public static IEnumerable<TItem> DistinctBy<TItem, TProperty>(this IEnumerable<TItem> enumerable, Func<TItem, TProperty> propertySelector)
        {
            return enumerable.GroupBy(propertySelector).Select(g => g.First());
        }
        
        /// <summary>
        /// Checks whether a list is a copy of another list. 
        /// </summary>
        /// <typeparam name="TItem">The type of an item. </typeparam>
        /// <param name="list1">The first list. </param>
        /// <param name="list2">The second list. </param>
        /// <returns>Returns true if the list is a copy. </returns>
        public static bool IsCopyOf<TItem>(this IList<TItem> list1, IList<TItem> list2)
        {
            if (list1 == null && list2 == null)
                return true;

            if (list1 == null)
                return false;
            if (list2 == null)
                return false; 

            if (list1.Any(a => !list2.Contains(a)))
                return false;
            if (list2.Any(a => !list1.Contains(a)))
                return false;
            return true; 
        }

        /// <summary>
        /// Returns a shuffled list. 
        /// </summary>
        /// <typeparam name="TItem">The type of an item. </typeparam>
        /// <param name="enumerable">The enumerable. </param>
        /// <returns>Returns the shuffled list. </returns>
        public static IEnumerable<TItem> Shuffle<TItem>(this IEnumerable<TItem> enumerable)
        {
            var rand = new Random((int)DateTime.Now.Ticks);
            return enumerable.Select(t => new KeyValuePair<int, TItem>(rand.Next(), t)).
                OrderBy(pair => pair.Key).Select(pair => pair.Value).ToList();
        }

        /// <summary>
        /// Takes random elements from a list. 
        /// </summary>
        /// <typeparam name="TItem">The type of an item. </typeparam>
        /// <param name="list">The list. </param>
        /// <param name="amount">The amount of items to take. </param>
        /// <returns>Returns the randomly taken items. </returns>
        public static IList<TItem> TakeRandom<TItem>(this IList<TItem> list, int amount)
        {
            list = new List<TItem>(list);

            var count = list.Count;
            var output = new List<TItem>();
            var rand = new Random((int)DateTime.Now.Ticks);
            for (var i = 0; (0 < count) && (i < amount); i++)
            {
                var index = rand.Next(count);
                var item = list[index];
                output.Add(item);
                list.RemoveAt(index);
                count--;
            }
            return output;
        }

        public static T MinObject<T, U>(this IEnumerable<T> list, Func<T, U> selector)
            where T : class
            where U : IComparable
        {
            U resultValue = default(U);
            T result = null;
            foreach (var t in list)
            {
                var value = selector(t);
                if (result == null || value.CompareTo(resultValue) < 0)
                {
                    result = t;
                    resultValue = value;
                }
            }
            return result;
        }

        public static T MaxObject<T, U>(this IEnumerable<T> list, Func<T, U> selector)
            where T : class
            where U : IComparable
        {
            U resultValue = default(U);
            T result = null;
            foreach (var t in list)
            {
                var value = selector(t);
                if (result == null || value.CompareTo(resultValue) > 0)
                {
                    result = t;
                    resultValue = value;
                }
            }
            return result;
        }

        public static IList<T> MiddleElements<T>(this IList<T> list, int count)
        {
            if (list.Count < count)
                return null;
            if (list.Count == count)
                return list.ToList();

            var output = new List<T>();
            var startIndex = list.Count/2 - count/2;
            for (var i = 0; i < count; i++)
                output.Add(list[startIndex + i]);
            return output; 
        }
    }
}
