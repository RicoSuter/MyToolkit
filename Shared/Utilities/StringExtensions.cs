using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace MyToolkit.Utilities
{
	public static class StringExtensions
	{
		public static Dictionary<string, string> GetConverterParameters(this string text)
		{
			var output = new Dictionary<string, string>();
			foreach (var item in text.Split(','))
			{
				var arr = item.Split(':');
				if (arr.Length == 2)
					output[arr[0].ToLower()] = arr[1];
				else
					output[arr[0].ToLower()] = ""; 
			}
			return output; 
		}

		public static string RemoveHtmlTags(this string text)
		{
			return Regex.Replace(text, "<[^>]*>", string.Empty);
		}

		public static string RemoveHtmlLinks(this string text)
		{
			return Regex.Replace(text, "<a[^>]*>[^<]*</a>", string.Empty);
		}

		public static string ConvertUTF8Characters(this string text)
		{
			return text.
				Replace("\u008B", "‹").
				Replace("\u009B", "›");
		}

		public static string TruncateWithoutChopping(this string text, int length)
		{
			if (text == null || text.Length < length)
				return text;

			var index = text.LastIndexOf(" ", length, StringComparison.Ordinal);
			return string.Format("{0}...", text.Substring(0, (index > 0) ? index : text.Length).Trim());
		}

		public static string TrimStart(this string input, string trimString)
		{
			var result = input;
			while (result.StartsWith(trimString))
				result = result.Substring(trimString.Length);
			return result;
		}

		public static string TrimEnd(this string input, string trimString)
		{
			var result = input;
			while (result.EndsWith(trimString))
				result = result.Substring(0, result.Length - trimString.Length);
			return result;
		}

		public static string Trim(this string input, string trimString)
		{
			return input.TrimStart(trimString).TrimEnd(trimString);
		}

		public static string ExtractLocalizedString(this string input, string language = null)
		{
			if (input == null)
				return null; 

			if (language == null)
				language = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

			var lines = input.Split(';');
			foreach (var line in lines.Select(l => l.Trim(' ')))
			{
				if (line.StartsWith(language + ":"))
					return line.Substring(3).Trim(' ');
			}
			return input.Contains(":") ? lines[0].Substring(3).Trim(' ') : input;
		}
	}
}
