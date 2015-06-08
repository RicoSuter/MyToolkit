using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MyToolkit.Converters
{
	public class LongListSelectorHasItemsConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value == null || !(bool)value
				? Application.Current.Resources["PhoneChromeBrush"]
				: Application.Current.Resources["PhoneAccentBrush"];
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
