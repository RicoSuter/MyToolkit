//-----------------------------------------------------------------------
// <copyright file="ImageBackgroundColorConverter.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#if !WINRT && !WPF

using System;
using System.Windows.Data;
using System.Windows.Media;
using MyToolkit.Environment;

namespace MyToolkit.Converters
{
	public class ImageBackgroundColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return new SolidColorBrush(!String.IsNullOrEmpty((String)value) ? Colors.Transparent : Environment.Resources.PhoneInactiveColor); 
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}

#endif