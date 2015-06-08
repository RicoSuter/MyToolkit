using System;
using System.Collections;
using System.Collections.Generic;

#if !WINRT
using System.Windows.Data;
#else
using Windows.UI.Xaml.Data;
#endif

namespace MyToolkit.Converters
{
	public class DictionaryItemConverter : IValueConverter
	{
#if !WINRT
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value == null)
				return "";

			if (parameter == null)
				parameter = "datetime";

			switch (parameter.ToString().ToLower())
			{
				case "date": return ((DateTime)value).ToShortDateString();
				case "time": return ((DateTime)value).ToShortTimeString();
				default: return ((DateTime)value).ToShortDateString() + " " + ((DateTime)value).ToShortTimeString();
			}
		}
#else
		public object Convert(object value, Type typeName, object parameter, string language)

		{
			var dict = (IDictionary)value;
			var path = parameter.ToString().Split('.');
			
			for (var i = 0; i < path.Length; i++)
			{
				var key = path[i];
				if (i < path.Length - 1)
				{
					if (dict.Contains(key) && dict[key] is IDictionary)
						dict = (IDictionary)dict[key];
					else
						return null; 
				}
				else
				{
					if (dict.Contains(key))
						return dict[key];
					return null; 
				}
			}
			return null; 
		}
#endif

#if !WINRT
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
#else
        public object ConvertBack(object value, Type typeName, object parameter, string language)
#endif
		{
			throw new NotImplementedException();
		}
    }
}
