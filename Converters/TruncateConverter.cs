using System;
using System.Windows.Data;

namespace MyToolkit.Converters
{
	public class TruncateConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var maxLength = int.Parse((string)parameter);
			var text = (string)value;
			if (text.Length > maxLength)
				return text.Substring(0, maxLength) + "...";
			return text; 
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
