using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace MyToolkit.Utilities
{
	public static class EnumerableExtensions
	{
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
