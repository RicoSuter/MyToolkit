using System;
using System.Collections;
using System.Windows;
using System.Windows.Data;

namespace MyToolkit.Converters
{
	public class VisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is bool)
				return (bool)value ? Visibility.Visible : Visibility.Collapsed;
			if (value is string)
				return !String.IsNullOrEmpty((string)value)  ? Visibility.Visible : Visibility.Collapsed;
			if (value is IList)
				return ((IList)value).Count > 0 ? Visibility.Visible : Visibility.Collapsed;
			if (value is int)
				return (int)value > 0 ? Visibility.Visible : Visibility.Collapsed;
			if (value is Visibility)
				return value;

			return value != null ? Visibility.Visible : Visibility.Collapsed; 
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
