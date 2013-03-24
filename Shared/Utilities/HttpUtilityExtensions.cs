using System;
using System.Collections.Generic;

namespace MyToolkit.Utilities
{
	public static class HttpUtilityExtensions
	{
		public static Dictionary<string, string> ParseQueryString(string queryString)
		{
			var dict = new Dictionary<string, string>();
			foreach (var s in queryString.Split('&'))
			{
				var index = s.IndexOf('=');
				if (index != -1 && index + 1 < s.Length)
				{
					var key = s.Substring(0, index);
					var value = Uri.UnescapeDataString(s.Substring(index + 1));
					dict.Add(key, value);
				}
			}
			return dict;
		}
	}
}
