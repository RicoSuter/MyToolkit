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




		public static bool Between(this DateTime dt, DateTime start, DateTime end)
		{
			return start <= dt && dt < end; 
		}

		public static bool Between(this DateTime? dt, DateTime start, DateTime end)
		{
			return dt.HasValue ? dt.Value.Between(start, end) : false;
		}

		public static bool Between(this DateTime dt, DateTime start, DateTime? end)
		{
			return start <= dt && (end == null || dt < end.Value);
		}

		public static bool Between(this DateTime? dt, DateTime start, DateTime? end)
		{
			return dt.HasValue ? dt.Value.Between(start, end) : false;
		}

		public static bool Between(this DateTime dt, DateTime? start, DateTime? end)
		{
			return (end == null || start.Value <= dt) && (end == null || dt < end.Value);
		}

		public static bool Between(this DateTime? dt, DateTime? start, DateTime? end)
		{
			return dt.HasValue ? dt.Value.Between(start, end) : false;
		}
	}
}
