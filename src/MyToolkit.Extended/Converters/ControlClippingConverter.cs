//-----------------------------------------------------------------------
// <copyright file="ControlClippingConverter.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#if !WINRT

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MyToolkit.Converters
{
	public class ControlClippingConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var ctrl = (FrameworkElement) value;
			return new Rect(0, 0, ctrl.ActualWidth, ctrl.ActualHeight);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}

#endif