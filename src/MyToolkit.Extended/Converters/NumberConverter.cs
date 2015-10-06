//-----------------------------------------------------------------------
// <copyright file="NumberConverter.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

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
#if WINRT
        public object Convert(object value, Type targetType, object parameter, string language)
#else
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
#endif
        {
            if (value is bool)
                value = (bool)value ? "1" : "0";

            if (targetType == typeof(double))
                return double.Parse(value.ToString());

            if (targetType == typeof(decimal))
                return decimal.Parse(value.ToString());

            if (targetType == typeof(int))
                return int.Parse(value.ToString());

            if (targetType == typeof(string))
            {
                if (value == null)
                    return "";

                var number = double.Parse(value.ToString());
                if (double.IsNaN(number) || double.IsInfinity(number))
                    number = 0;

                return string.Format("{0:F" + parameter + "}", number);
            }

            return null;
        }

#if WINRT
        public object ConvertBack(object value, Type targetType, object parameter, string language)
#else
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
#endif
        {
            throw new NotSupportedException();
        }
    }
}
