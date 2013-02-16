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
	public class NumberConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if (value is bool)
				value = (bool) value ? "1" : "0";

			if (targetType == typeof(double))
				return double.Parse(value.ToString());
			if (targetType == typeof(decimal))
				return decimal.Parse(value.ToString());
			if (targetType == typeof(int))
				return int.Parse(value.ToString());
			return null; 
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}
