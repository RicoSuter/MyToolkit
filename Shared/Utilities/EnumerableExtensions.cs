using System;
using System.Collections.Generic;
using System.Linq;

namespace MyToolkit.Utilities
{
	public static class EnumerableExtensions
	{

#if !WP7
		public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			var keys = new HashSet<TKey>();
			foreach (var element in source)
			{
				if (keys.Add(keySelector(element)))
					yield return element;
			}
		}
#endif

		public static bool IsCopyOf<T>(this IList<T> list1, IList<T> list2)
		{
			if (list1.Any(a => !list2.Contains(a)))
				return false;
			if (list2.Any(a => !list1.Contains(a)))
				return false;
			return true; 
		}

		public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
		{
			var rand = new Random((int)DateTime.Now.Ticks);
			return source.Select(t => new KeyValuePair<int, T>(rand.Next(), t)).
				OrderBy(pair => pair.Key).Select(pair => pair.Value).ToList();
		}

		public static IList<T> TakeRandom<T>(this IList<T> source, int amount)
		{
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
	}
}
