//-----------------------------------------------------------------------
// <copyright file="TruncateConverter.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using MyToolkit.Utilities;

#if !WINRT
using System.Windows.Data;
#else
using Windows.UI.Xaml.Data;
#endif

namespace MyToolkit.Converters
{
    public class TruncateConverter : IValueConverter
    {
#if !WINRT
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
#else
        public object Convert(object value, Type typeName, object parameter, string language)
#endif		
        {
            var maxLength = int.Parse((string)parameter);
            if (value != null)
                return value.ToString().TruncateWithoutChopping(maxLength);
            return null; 
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
