using System;
using System.Windows;
using System.Windows.Data;

namespace MyToolkit.Converters
{
	public class TypeVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return value.GetType().Name == (string)parameter ? Visibility.Visible : Visibility.Collapsed; 
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
