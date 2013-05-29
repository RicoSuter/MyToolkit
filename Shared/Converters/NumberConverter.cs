using System;

#if !WINRT
using System.Windows.Data;
#else
using Windows.UI.Xaml.Data;
#endif


namespace MyToolkit.Converters
{
	public class NumberConverter : IValueConverter
	{
#if !WINRT
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
#else
        public object Convert(object value, Type typeName, object parameter, string language)
#endif
		{
			return string.Format("{0:F" + parameter + "}", double.Parse(value.ToString()));
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
