using System;
using MyToolkit.Utilities;

#if !METRO
using System.Windows.Data;
#else
using Windows.UI.Xaml.Data;
#endif

namespace MyToolkit.Converters
{
	public class TruncateConverter : IValueConverter
	{
#if !METRO
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
#else
        public object Convert(object value, Type typeName, object parameter, string language)
#endif		
		{
			var maxLength = int.Parse((string)parameter);
			return value.ToString().TruncateWithoutChopping(maxLength);
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
