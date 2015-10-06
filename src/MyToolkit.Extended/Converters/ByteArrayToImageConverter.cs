//-----------------------------------------------------------------------
// <copyright file="ByteArrayToImageConverter.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.IO;
using System.Linq;

#if !WINRT
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
#else
using Windows.UI.Xaml.Media.Imaging;
using System.Reflection;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml;
#endif

namespace MyToolkit.Converters
{
    public class ByteArrayToImageConverter : IValueConverter
    {
#if !WINRT
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
#else
        public object Convert(object value, Type typeName, object parameter, string language)
#endif
        {
            if (value is byte[])
            {
                var array = (byte[])value;
                if (array.Length > 0)
                {
                    try
                    {
                        var stream = new MemoryStream();
                        stream.Write(array, 0, array.Length);

                        var img = new BitmapImage();
#if WINRT
                        img.SetSource(stream.AsRandomAccessStream());
#elif WPF
                        img.StreamSource = stream;
#else
                        img.SetSource(stream);
#endif
                        return img;
                    }
                    catch { }
                }
            }
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
