using System;
using System.Windows;
using System.Windows.Data;

namespace MyToolkit.Converters
{
	public class NotConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (targetType == typeof(Visibility))
			{
				var c = new VisibilityConverter();
				var r = c.Convert(value, targetType, parameter, culture);
				return ((Visibility)r) == Visibility.Visible
						? Visibility.Collapsed : Visibility.Visible;
			}
			
			if(targetType == typeof(bool))
			{
				if (value == null)
					return true;
				return !((bool) value);
			}

			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
