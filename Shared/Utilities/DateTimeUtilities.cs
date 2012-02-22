using System;

namespace MyToolkit.Utilities
{
	public class DateTimeUtilities
	{
		public static DateTime FromUnixTimeStamp(double unixTimeStamp)
		{
			var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
			dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
			return dtDateTime;
		}
	}
}
