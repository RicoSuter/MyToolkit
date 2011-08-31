using System;
using System.Windows.Data;
using System.Windows.Media;
using MyToolkit.Phone;

namespace MyToolkit.Converters
{
	public class ReadColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return new SolidColorBrush((bool)value ? Colors.Transparent : Resources.PhoneAccentColor); 
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
