using System;

#if !METRO
using System.Windows;
using System.Windows.Data;
#else
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml;
#endif

namespace MyToolkit.Converters
{
	public class NotConverter : IValueConverter
	{
#if !METRO
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
#else
		public object Convert(object value, string targetType, object parameter, string language)
#endif		
		{
			if (targetType == typeof(Visibility).FullName)
			{
				var c = new VisibilityConverter();
#if !METRO
				var r = c.Convert(value, targetType, parameter, culture);
#else
				var r = c.Convert(value, targetType, parameter, language);
#endif
				return ((Visibility)r) == Visibility.Visible
						? Visibility.Collapsed : Visibility.Visible;
			}

			if (targetType == typeof(bool).FullName)
			{
				if (value == null)
					return true;
				return !((bool) value);
			}

			return null;
		}

#if !METRO
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
#else
		public object ConvertBack(object value, string typeName, object parameter, string language)
#endif
		{
			throw new NotSupportedException();
		}
	}
}
