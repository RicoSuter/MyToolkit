using System;

namespace MyToolkit.Utilities
{
	public static class DateTimeExtensions
	{
		public static DateTime ToStartOfDay(this DateTime dt)
		{
			return new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0);
		}

		public static DateTime ToEndOfDay(this DateTime dt)
		{
			return new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59, 999);
		}

		public static DateTime? ToStartOfDay(this DateTime? dt)
		{
			return dt.HasValue ? dt.Value.ToStartOfDay() : (DateTime?)null;
		}

		public static DateTime? ToEndOfDay(this DateTime? dt)
		{
			return dt.HasValue ? dt.Value.ToEndOfDay() : (DateTime?)null;
		}
	}
}
