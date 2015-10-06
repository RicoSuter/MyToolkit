//-----------------------------------------------------------------------
// <copyright file="UpperTextConverter.cs" company="MyToolkit">
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
    public class UpperTextConverter : IValueConverter
    {
#if !WINRT
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
#else
        public object Convert(object value, Type typeName, object parameter, string language)
#endif		
        {
            return value.ToString().ToUpper();
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
