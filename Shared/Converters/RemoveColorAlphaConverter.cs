using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using MyToolkit.Utilities;

namespace MyToolkit.Converters
{
	public class RemoveColorAlphaConverter : IValueConverter
	{
		private static Dictionary<Color, SolidColorBrush> colors = null; 
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var color = (Color) value; 
			if (colors == null)
				colors = new Dictionary<Color, SolidColorBrush>();

			if (colors.ContainsKey(color))
				return colors[color];

			colors[color] = new SolidColorBrush(ColorUtility.RemoveAlpha(color, 
				(Color) Application.Current.Resources["PhoneBackgroundColor"]));
			return colors[color];
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
