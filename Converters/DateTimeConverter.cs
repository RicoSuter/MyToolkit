using System;
using System.Windows.Data;

namespace MyToolkit.Converters
{
	public class DateTimeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if(value == null)
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

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
