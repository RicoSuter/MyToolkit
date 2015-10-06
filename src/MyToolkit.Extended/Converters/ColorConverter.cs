//-----------------------------------------------------------------------
// <copyright file="ColorConverter.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

#if WINRT

using System;
using MyToolkit.Utilities;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace MyToolkit.Converters
{
    /// <summary>
    /// Converts a HEX string, color or brush to a HEX string, color or brush. 
    /// </summary>
    public class ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Color color;
            if (value is Color)
                color = (Color)value;
            else if (value is String)
                color = ColorUtilities.FromString((string)value);
            else if (value is SolidColorBrush)
                color = ((SolidColorBrush) value).Color;

            if (parameter != null)
            {
                var parameters = parameter.ToString().GetConverterParameters();
                if (parameters.ContainsKey("alpha"))
                    color = ColorUtilities.ChangeAlpha(color, parameters["alpha"]);
            }

            if (targetType == typeof(Brush))
                return new SolidColorBrush(color);
            if (targetType == typeof(Color))
                return color; 
            if (targetType == typeof(string))
                return ColorUtilities.ToHex(color, true);
            return null; 
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

#endif