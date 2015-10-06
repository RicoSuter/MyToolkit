//-----------------------------------------------------------------------
// <copyright file="VisibilityConverter.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections;

#if !WINRT
using System.Windows;
using System.Windows.Data;
#else
using System.Reflection;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml;
#endif

namespace MyToolkit.Converters
{
    public class VisibilityConverter : IValueConverter
    {
#if !WINRT
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
#else
        public object Convert(object value, Type targetType, object parameter, string language)
#endif
        {
            var isVisible = value != null;

            if (value is bool)
                isVisible = (bool)value;
            if (value is string)
                isVisible = !String.IsNullOrEmpty((string)value);
            if (value is IList)
            {
                var list = (IList)value;
                if (list.Count == 0)
                    isVisible = false;
                else
                {
                    isVisible = true; 
                    if (parameter is string)
                    {
                        var p = parameter.ToString();
                        if (p.StartsWith("CheckAll:"))
                        {
                            p = p.Substring(9);
                            foreach (var item in list)
                            {
#if WINRT
                                var property = item.GetType().GetRuntimeProperty(p);
#else
                                var property = item.GetType().GetProperty(p);
#endif
                                if (property != null)
                                {
                                    var val = property.GetValue(item, null);
                                    if (!(bool)Convert(val, typeof(bool), null, null))
                                    {
                                        isVisible = false;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (value is int)
                isVisible = (int)value > 0;
            if (value is Visibility)
                isVisible = (Visibility)value == Visibility.Visible;

            if (targetType == typeof(Visibility))
                return isVisible ? Visibility.Visible : Visibility.Collapsed;
            return isVisible; 
        }

#if !WINRT
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
#else
        public object ConvertBack(object value, Type typeName, object parameter, string language)
#endif
        {
            throw new NotSupportedException();
        }
    }
}
