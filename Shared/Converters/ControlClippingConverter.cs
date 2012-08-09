using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MyToolkit.Converters
{
	public class ControlClippingConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var ctrl = (FrameworkElement) value;
			return new Rect(0, 0, ctrl.ActualWidth, ctrl.ActualHeight);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
