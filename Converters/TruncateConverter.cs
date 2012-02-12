using System;
using System.Windows.Data;
using MyToolkit.Utilities;

namespace MyToolkit.Converters
{
	public class TruncateConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var maxLength = int.Parse((string)parameter);
			return value.ToString().TruncateWithoutChopping(maxLength);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
