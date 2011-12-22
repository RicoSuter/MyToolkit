using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using MyToolkit.Utilities;

namespace MyToolkit.Converters
{
	public class RemoveColorAlphaConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return new SolidColorBrush(ColorUtility.RemoveAlpha((Color) value, (Color) Application.Current.Resources["PhoneBackgroundColor"]));
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
