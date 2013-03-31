using System;
using System.Linq;

#if !WINRT
using System.Windows;
using System.Windows.Data;
#else
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml;
using System.Reflection;
#endif

namespace MyToolkit.Converters
{
	public class TypeVisibilityConverter : IValueConverter
	{
#if !WINRT
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			
			var type = parameter.ToString().ToLower().Split(',');
			var isOfType = value != null && type.Contains(value.GetType().Name.ToLower());
			if (targetType == typeof (bool))
				return isOfType; 
			return isOfType ? Visibility.Visible : Visibility.Collapsed; 
		}
#else
        public object Convert(object value, Type typeName, object parameter, string language)
		{
			return value.GetType().GetTypeInfo().Name == (string)parameter ? Visibility.Visible : Visibility.Collapsed; 
		}
#endif

#if !WINRT
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
#else
        public object ConvertBack(object value, Type typeName, object parameter, string language)
#endif
		{
			throw new NotSupportedException();
		}
	}
}
