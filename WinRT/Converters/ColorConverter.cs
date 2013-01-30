using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MyToolkit.Utilities;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace MyToolkit.Converters
{
	public class ColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			Color color;
			if (value is Color)
				color = (Color)value;
			else if (value is String)
				color = ColorUtility.FromString((string)value);
			else if (value is SolidColorBrush)
				color = ((SolidColorBrush) value).Color;

			if (parameter != null)
			{
				var parameters = parameter.ToString().GetConverterParameters();
				if (parameters.ContainsKey("alpha"))
					color = ColorUtility.ChangeAlpha(color, parameters["alpha"]);
			}

			if (targetType == typeof(Brush))
				return new SolidColorBrush(color);
			if (targetType == typeof(Color))
				return color; 
			if (targetType == typeof(string))
				return ColorUtility.ToHex(color, true);
			return null; 
		}
		
		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}
