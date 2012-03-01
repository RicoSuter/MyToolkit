using System;

#if !METRO
	using System.Windows.Data;
#else
	using Windows.UI.Xaml.Data;
	using Windows.Globalization.DateTimeFormatting;
#endif

namespace MyToolkit.Converters
{
	public class DateTimeConverter : IValueConverter
	{
#if !METRO
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value == null)
				return "";

			if (parameter == null)
				parameter = "datetime";

			switch (parameter.ToString().ToLower())
			{
				case "date": return ((DateTime)value).ToShortDateString();
				case "time": return ((DateTime)value).ToShortTimeString();
				default: return ((DateTime)value).ToShortDateString() + " " + ((DateTime)value).ToShortTimeString();
			}
		}
#else
		public object Convert(object value, Type typeName, object parameter, string language)

		{
			if (value == null)
				return "";

			if (parameter == null)
				parameter = "datetime";

			switch (parameter.ToString().ToLower())
			{
				case "date": return new DateTimeFormatter(YearFormat.Default, MonthFormat.Default, DayFormat.Default, DayOfWeekFormat.None).Format(((DateTime)value));
				case "time": return new DateTimeFormatter(HourFormat.Default, MinuteFormat.Default, SecondFormat.Default).Format(((DateTime)value));
				default: return new DateTimeFormatter(YearFormat.Default, MonthFormat.Default, DayFormat.Default, DayOfWeekFormat.None).Format(((DateTime)value)) + " " +
					new DateTimeFormatter(HourFormat.Default, MinuteFormat.Default, SecondFormat.Default).Format(((DateTime)value));
			}
		}
#endif

#if !METRO
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
#else
        public object ConvertBack(object value, Type typeName, object parameter, string language)
#endif
		{
			var text = value.ToString();
			if (String.IsNullOrEmpty(text))
				return null; 
			return DateTime.Parse(text);
		}
    }
}
