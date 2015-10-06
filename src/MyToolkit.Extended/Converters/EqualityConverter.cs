//-----------------------------------------------------------------------
// <copyright file="EqualityConverter.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Linq;

#if WINRT
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml;
#else
using System.Windows;
using System.Windows.Data;
#endif

namespace MyToolkit.Converters
{
    /// <summary>
    /// Returns a bool whether the objects string representation equals to the converter parameter. 
    /// The parameter can have multiple values devided by comma. 
    /// It may begin with ! for not-equality, but it cannot have multiple values.  
    /// </summary>
    public class EqualityConverter : IValueConverter
    {
#if !WINRT
        public object Convert(object valueObject, Type targetType, object parameterObject, System.Globalization.CultureInfo culture)
#else
        public object Convert(object valueObject, Type targetType, object parameterObject, string language)
#endif
        {
            var matches = false;

            var parameter = parameterObject != null ? parameterObject.ToString() : string.Empty;
            var value = valueObject != null ? valueObject.ToString() : string.Empty;

            if (parameter.StartsWith("!"))
                matches = parameter.Substring(1) != value;
            else
                matches = parameter.Split(',').Contains(value);

            if (targetType == typeof(Visibility))
                return matches ? Visibility.Visible : Visibility.Collapsed;
            return matches; 

        }

#if !WINRT
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
#else
        public object ConvertBack(object value, Type targetType, object parameter, string language)
#endif
        {
            throw new NotSupportedException();
        }
    }
}
