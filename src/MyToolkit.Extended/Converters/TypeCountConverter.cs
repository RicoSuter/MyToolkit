using System;
using System.Collections;
using System.Linq;

#if !WINRT
using System.Windows;
using System.Windows.Data;
#else
using System.Reflection;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml;
#endif


namespace MyToolkit.Converters
{
	public class TypeCountConverter : IValueConverter
	{
#if !WINRT
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
#else
        public object Convert(object value, Type targetType, object parameter, string language)
#endif
		{
			var type = parameter.ToString().ToLower().Split(',');
			if (value is IEnumerable)
			{
				var list = (IEnumerable) value;
				var count = list.OfType<object>().Count(i => type.Contains(i.GetType().Name.ToLower()));
				if (targetType == typeof (Visibility))
					return count > 0 ? Visibility.Visible : Visibility.Collapsed;
				return count;
			}
			return null; 
		}

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
