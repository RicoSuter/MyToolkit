using System;
using MyToolkit.Resources;
using System.Linq;

#if !METRO
	using System.Windows.Data;
#else
	using Windows.UI.Xaml.Data;
	using Windows.Globalization.DateTimeFormatting;
#endif

namespace MyToolkit.Converters
{
	public class TimeSpanConverter : IValueConverter
	{
#if !METRO
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
#else
		public object Convert(object value, Type typeName, object parameter, string language)
#endif
		{
			if (value == null)
				return "";

			var span = (TimeSpan)value;

			var parameters = parameter != null ? 
				parameter.ToString().ToLower().Split(',').Select(p => p.Trim(' ')).ToArray() : 
				new string[] {};

			if (parameters.Contains("days"))
			{
				if (!parameters.Contains("plain"))
				{
					if (span.TotalDays > 1.0)
						return Math.Round(span.TotalDays, 2) + " " + Strings.Days;
					return Math.Round(span.TotalDays, 2) + " " + Strings.Day;
				}
				return Math.Round(span.TotalDays, 2);
			}

			if (!parameters.Contains("plain"))
			{
				var hours = (int) span.TotalHours;
				var minutes = (int) Math.Abs(span.Minutes);

				return hours + " " + (hours != 1 ? Strings.Hours : Strings.Hour) + " " + 
					minutes + " " + (minutes != 1 ? Strings.Minutes : Strings.Minute);
			}
			return ((int)span.TotalHours).ToString("D2") + ":" + Math.Abs(span.Minutes).ToString("D2");
		}


#if !METRO
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
#else
        public object ConvertBack(object value, Type typeName, object parameter, string language)
#endif
		{
			var text = value.ToString();
			if (String.IsNullOrEmpty(text))
				return null; 
			return TimeSpan.Parse(text);
		}
    }
}
