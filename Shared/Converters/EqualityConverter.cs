using System;
using System.Collections;

#if !WINRT
using System.Windows;
using System.Windows.Data;
#else
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml;
#endif


namespace MyToolkit.Converters
{
	public class EqualityConverter : IValueConverter
	{
#if !WINRT
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
#else
		public object Convert(object value, Type targetType, object parameter, string language)
#endif
		{
			var inverse = false;
			if (parameter != null && parameter.ToString().StartsWith("!"))
			{
				inverse = true; 
				parameter = parameter.ToString().Substring(1);
			}

			var matches = value != null && value.ToString() == parameter.ToString(); 
			if (targetType == typeof(Visibility))
				return !inverse ? 
					(matches ? Visibility.Visible : Visibility.Collapsed) : 
					(matches ? Visibility.Collapsed : Visibility.Visible);
			return !inverse ? matches : !matches;
		}

#if !WINRT
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
#else
		public object ConvertBack(object value, Type targetType, object parameter, string language)
#endif
		{
			throw new NotSupportedException();
		}
	}
}
