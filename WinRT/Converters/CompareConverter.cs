using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace MyToolkit.Converters
{
	public class CompareConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			var left = value.ToString();
			var right = parameter.ToString(); 
			
			if (targetType == typeof(Visibility))
				return left == right ? Visibility.Visible : Visibility.Collapsed;
			return left == right; 
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}
