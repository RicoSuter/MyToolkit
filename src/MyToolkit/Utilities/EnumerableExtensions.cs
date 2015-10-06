//-----------------------------------------------------------------------
// <copyright file="EnumerableExtensions.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace MyToolkit.Utilities
{
    /// <summary>Provides extension methods for enumerations. </summary>
    public static class EnumerableExtensions
    {
        /// <summary>Provides ordering by two expressions. Use this method instaed of OrderBy(...).ThenBy(...) as it calls ThenBy only if necessary. </summary>
        public static IEnumerable<TSource> OrderByThenBy<TSource, TKey1, TKey2>(this IEnumerable<TSource> source, Func<TSource, TKey1> orderBy, Func<TSource, TKey2> thenBy)
        {
            var sorted = source
                .Select(s => new Tuple<TSource, TKey1>(s, orderBy(s)))
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

        /// <summary>Removes equal objects by specifing the comparing key. </summary>
        /// <typeparam name="TSource">The type of an item. </typeparam>
        /// <typeparam name="TKey">The type of the key. </typeparam>
        /// <param name="source">The source enumerable. </param>
        /// <param name="keySelector">The key selector. </param>
        /// <returns>The filtered enumerable. </returns>
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            return source.GroupBy(keySelector).Select(g => g.First());
        }

        /// <summary>Returns true if the second list contains exactly the same items in the same order or is equal. </summary>
        /// <typeparam name="T">The item type. </typeparam>
        /// <param name="list1">The first list. </param>
        /// <param name="list2">The second list. </param>
        /// <returns></returns>
        public static bool IsCopyOf<T>(this IList<T> list1, IList<T> list2)
        {
            if (list1 == null && list2 == null)
                return true;
            if (Equals(list1, list2))
                return true;

            if (list1 == null)
                return false;
            if (list2 == null)
                return false;

            if (list1.Count != list2.Count)
                return false;

            // Has same order
            for (int i = 0; i < list1.Count; i++)
            {
                if (!Equals(list1[i], list2[i]))
                    return false;
            }

            // Has same elements
            if (list1.Any(a => !list2.Contains(a)))
                return false;
            if (list2.Any(a => !list1.Contains(a)))
                return false;

            return true;
        }

        /// <summary>Returns true if the second list contains exactly the same items or is equal. </summary>
        /// <typeparam name="T">The item type. </typeparam>
        /// <param name="list1">The first collection. </param>
        /// <param name="list2">The second collection. </param>
        /// <returns></returns>
        public static bool IsCopyOf<T>(this ICollection<T> list1, ICollection<T> list2)
        {
            if (list1 == null && list2 == null)
                return true;
            if (Equals(list1, list2))
                return true;

            if (list1 == null)
                return false;
            if (list2 == null)
                return false;

            if (list1.Count != list2.Count)
                return false;

            // Has same elements
            if (list1.Any(a => !list2.Contains(a)))
                return false;
            if (list2.Any(a => !list1.Contains(a)))
                return false;

            return true;
        }

        /// <summary>Returns a shuffled list. </summary>
        /// <typeparam name="T">The item type. </typeparam>
        /// <param name="source">The list to shuffle. </param>
        /// <returns>The shuffled list. </returns>
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            var rand = new Random((int)DateTime.Now.Ticks);
            return source.Select(t => new KeyValuePair<int, T>(rand.Next(), t)).
                OrderBy(pair => pair.Key).Select(pair => pair.Value).ToList();
        }

        /// <summary>Takes random items from the given list. </summary>
        /// <typeparam name="T">The item type. </typeparam>
        /// <param name="source">The list to take the items from. </param>
        /// <param name="amount">The amount of items to take. </param>
        /// <returns>The randomly taken items. </returns>
        public static IList<T> TakeRandom<T>(this IList<T> source, int amount)
        {
            source = new List<T>(source);

            var count = source.Count;
            var output = new List<T>();
            var rand = new Random((int)DateTime.Now.Ticks);
            for (var i = 0; (0 < count) && (i < amount); i++)
            {
                var index = rand.Next(count);
                var item = source[index];
                output.Add(item);
                source.RemoveAt(index);
                count--;
            }
            return output;
        }

        /// <summary>Takes the minimal object from a list. </summary>
        /// <typeparam name="T">The item type. </typeparam>
        /// <typeparam name="U">The compared type. </typeparam>
        /// <param name="list">The list to search in. </param>
        /// <param name="selector">The selector of the object to compare. </param>
        /// <returns>The minimal object. </returns>
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

        /// <summary>Takes the maximum object from a list. </summary>
        /// <typeparam name="T">The item type. </typeparam>
        /// <typeparam name="TProperty">The compared type. </typeparam>
        /// <param name="list">The list to search in. </param>
        /// <param name="selector">The selector of the object to compare. </param>
        /// <returns>The maximum object. </returns>
        public static T MaxObject<T, TProperty>(this IEnumerable<T> list, Func<T, TProperty> selector)
            where T : class
            where TProperty : IComparable
        {
            TProperty resultValue = default(TProperty);
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

        /// <summary>Gets a specified amount of items in the middle of a list. </summary>
        /// <typeparam name="T">The item type. </typeparam>
        /// <param name="list">The list. </param>
        /// <param name="count">The amount of items to retrieve. </param>
        /// <returns>The middle items. </returns>
        public static IList<T> MiddleElements<T>(this IList<T> list, int count)
        {
            if (list.Count < count)
                return null;
            if (list.Count == count)
                return list.ToList();

            var output = new List<T>();
            var startIndex = list.Count / 2 - count / 2;
            for (var i = 0; i < count; i++)
                output.Add(list[startIndex + i]);
            return output;
        }

        /// <summary>Partitions an enumerable into blocks of a given size.</summary>
        /// <typeparam name="T">The item type. </typeparam>
        /// <param name="source">The source enumeration.</param>
        /// <param name="blockSize">Size of the block.</param>
        /// <returns>The partitions. </returns>
        public static IEnumerable<IEnumerable<T>> Partition<T>(this IEnumerable<T> source, int blockSize)
        {
            var enumerator = source.GetEnumerator();
            while (enumerator.MoveNext())
                yield return GetNextPartition(enumerator, blockSize);
        }

        private static IEnumerable<T> GetNextPartition<T>(IEnumerator<T> enumerator, int blockSize)
        {
            do
            {
                yield return enumerator.Current;
            }
            while (--blockSize > 0 && enumerator.MoveNext());
        }
    }
}
