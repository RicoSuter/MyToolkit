using System;

namespace MyToolkit.Utilities
{
	public static class DateTimeUtilities
	{
		public static DateTime FromUnixTimeStamp(double unixTimeStamp)
		{
			var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
			dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
			return dtDateTime;
		}

		public static DateTime SetTimeTakeDate(this DateTime date, int hour, int minute, int second)
		{
			return new DateTime(date.Year, date.Month, date.Day, hour, minute, second);
		}
	}
}
