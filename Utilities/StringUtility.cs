using System;
using System.Net;
using System.Text.RegularExpressions;
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
	public static class StringUtility
	{
		public static string RemoveTags(string text)
		{
			return Regex.Replace(text, "<[^>]*>", string.Empty);
		}

		public static string RemoveLinks(string text)
		{
			return Regex.Replace(text, "<a[^>]*>[^<]*</a>", string.Empty);
		}
	}
}
