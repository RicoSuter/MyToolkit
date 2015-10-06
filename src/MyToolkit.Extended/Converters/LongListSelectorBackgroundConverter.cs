//-----------------------------------------------------------------------
// <copyright file="LongListSelectorBackgroundConverter.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#if WINRT

using System;
using System.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace MyToolkit.Converters
{
    public class LongListSelectorBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var group = (ICollection)value;
            var count = group.Count;

            return count == 0
                ? Application.Current.Resources["PhoneChromeBrush"]
                : Application.Current.Resources["PhoneAccentBrush"];
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

#endif