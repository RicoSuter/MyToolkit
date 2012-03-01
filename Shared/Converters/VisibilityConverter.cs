using System;
using System.Collections;

#if !METRO
using System.Windows;
using System.Windows.Data;
#else
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml;
#endif


namespace MyToolkit.Converters
{
	public class VisibilityConverter : IValueConverter
	{
#if !METRO
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
#else
        public object Convert(object value, Type typeName, object parameter, string language)
#endif		
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

#if !METRO
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
#else
        public object ConvertBack(object value, Type typeName, object parameter, string language)
#endif
		{
			throw new NotSupportedException();
		}
	}
}
