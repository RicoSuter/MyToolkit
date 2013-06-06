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
		public static string EscapeUriString(this string value)
		{
			const int limit = 32768;
			var sb = new StringBuilder();
			var loops = value.Length / limit;
			for (int i = 0; i <= loops; i++)
			{
				if (i < loops)
					sb.Append(Uri.EscapeDataString(value.Substring(limit * i, limit)));
				else
					sb.Append(Uri.EscapeDataString(value.Substring(limit * i)));
			}
			return sb.ToString();
		}

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

			var mapping = new Dictionary<string, string>();
			var position = 0;
			string key = null; 
			while (true)
			{
				if (key == null) // find language
				{
					var index = input.IndexOf(':', position);
					if (index == -1)
						break;

					key = input.Substring(position, index - position).Trim();
					position = index + 1; 
				}
				else
				{
					var semiIndex = input.IndexOf(';', position);
					var startQuoteIndex = input.IndexOf('"', position);

					if (startQuoteIndex != -1 && (semiIndex == -1 || startQuoteIndex < semiIndex))
					{
						position = startQuoteIndex + 1; 
						while (true)
						{
							position = input.IndexOf('"', position);
							if (position == -1)
								return "wrong_quoting";

							position++;
							if (input[position - 2] != '\\')
								break;
						}

						if (position == -1)
							break;

						mapping.Add(key, input.Substring(startQuoteIndex + 1, position - startQuoteIndex - 2).Replace("\\\"", "\"").Trim());
						position = input.IndexOf(';', position);

						if (position == -1)
							break;
						position++; 
					}
					else
					{
						if (semiIndex == -1)
						{
							mapping.Add(key, input.Substring(position).Trim());
							break; 
						}
						mapping.Add(key, input.Substring(position, semiIndex - position).Trim());
						position = semiIndex + 1;
					}

					key = null; 
				}
			}

			return mapping.Count > 0 ? (mapping.ContainsKey(language) ? mapping[language] : mapping.First().Value) : input; 
		}
	}
}
