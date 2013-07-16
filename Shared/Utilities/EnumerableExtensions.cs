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

		public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			return source.GroupBy(keySelector).Select(g => g.First());
		}
		
		public static bool IsCopyOf<T>(this IList<T> list1, IList<T> list2)
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

		public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
		{
			var rand = new Random((int)DateTime.Now.Ticks);
			return source.Select(t => new KeyValuePair<int, T>(rand.Next(), t)).
				OrderBy(pair => pair.Key).Select(pair => pair.Value).ToList();
		}

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
