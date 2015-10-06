//-----------------------------------------------------------------------
// <copyright file="ColorUtilities.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
#if !WINRT
using System.Windows.Media;
#else
using System.Reflection;
using Windows.UI;
#endif

namespace MyToolkit.Utilities
{
    /// <summary>Provides methods to manipulate <see cref="Color"/> objects. </summary>
    public class ColorUtilities
    {
        /// <summary>Blends the given foreground color with the background using the given alpha value. </summary>
        /// <param name="foreground">The foreground <see cref="Color"/>. </param>
        /// <param name="alpha">The alpha value. </param>
        /// <param name="background">The background <see cref="Color"/>. </param>
        /// <returns>The new <see cref="Color"/>. </returns>
        public static Color Mix(Color foreground, double alpha, Color background)
        {
            var diff = 1.0 - alpha;
            var color = Color.FromArgb(foreground.A,
                (byte)(foreground.R * alpha + background.R * diff),
                (byte)(foreground.G * alpha + background.G * diff),
                (byte)(foreground.B * alpha + background.B * diff));
            return color; 
        }

        /// <summary>Removes the transparency from the foreground color using the given background color. </summary>
        /// <param name="foreground">The foreground <see cref="Color"/>. </param>
        /// <param name="background">The background <see cref="Color"/>. </param>
        /// <returns>The color without transparency. </returns>
        public static Color RemoveAlpha(Color foreground, Color background)
        {
            if (foreground.A == 255)
                return foreground;

            var alpha = foreground.A / 255.0;
            var diff = 1.0 - alpha;
            return Color.FromArgb(255,
                (byte)(foreground.R * alpha + background.R * diff),
                (byte)(foreground.G * alpha + background.G * diff),
                (byte)(foreground.B * alpha + background.B * diff));
        }

        /// <summary>Changes the alpha channel of the <see cref="Color"/>. </summary>
        /// <param name="color">The <see cref="Color"/>. </param>
        /// <param name="alpha">The new alpha value. </param>
        /// <returns>The new <see cref="Color"/>. </returns>
        public static Color ChangeAlpha(Color color, byte alpha)
        {
            return Color.FromArgb(alpha, color.R, color.G, color.B);
        }

        /// <summary>Changes the alpha channel of the <see cref="Color"/>. </summary>
        /// <param name="color">The <see cref="Color"/>. </param>
        /// <param name="alpha">The new alpha value. </param>
        /// <returns>The new <see cref="Color"/>. </returns>
        public static Color ChangeAlpha(Color color, string alpha)
        {
            var value = UInt32.Parse(alpha, NumberStyles.HexNumber);
            return ChangeAlpha(color, (byte)(value & 0xff));
        }

        /// <summary>Converts a <see cref="Color"/> to HEX string. </summary>
        /// <param name="color">The <see cref="Color"/>. </param>
        /// <param name="includeAlpha">If false then #RRGGBB, true then #AARRGGBB. </param>
        /// <returns>The HEX string. </returns>
        public static string ToHex(Color color, bool includeAlpha = false)
        {
            if (includeAlpha)
                return "#" +
                    Convert.ToInt32(color.A).ToString("X2") +
                    Convert.ToInt32(color.R).ToString("X2") +
                    Convert.ToInt32(color.G).ToString("X2") +
                    Convert.ToInt32(color.B).ToString("X2");

            return "#" +
                Convert.ToInt32(color.R).ToString("X2") +
                Convert.ToInt32(color.G).ToString("X2") +
                Convert.ToInt32(color.B).ToString("X2");
        }

        /// <summary>Creates a <see cref="Color"/> from a HEX value. </summary>
        /// <param name="colorCode">The HEX in the form #RRGGBB or #AARRGGBB. </param>
        /// <returns>The <see cref="Color"/>. </returns>
        public static Color FromHex(string colorCode)
        {
            colorCode = colorCode.Replace("#", "");
            if (colorCode.Length == 6)
                colorCode = "FF" + colorCode;
            return FromHex(UInt32.Parse(colorCode, NumberStyles.HexNumber));
        }

        /// <summary>Creates a <see cref="Color"/> from a HEX value. </summary>
        /// <param name="argb">The HEX value. </param>
        /// <returns>The <see cref="Color"/>. </returns>
        public static Color FromHex(uint argb)
        {
            return Color.FromArgb((byte)((argb & -16777216) >> 0x18),
                (byte)((argb & 0xff0000) >> 0x10),
                (byte)((argb & 0xff00) >> 8),
                (byte)(argb & 0xff));
        }

#if WINRT

        /// <summary>Creates a <see cref="Color"/> from a <see cref="Colors"/> enum string. </summary>
        /// <param name="value">The color string. </param>
        /// <returns>The <see cref="Color"/>. </returns>
        public static Color FromString(string value)
        {
            var property = typeof(Colors).GetRuntimeProperty(value);
            if (property != null)
                return (Color)property.GetValue(null);
            return ColorUtilities.FromHex(value);
        }

#endif
    }

    [Obsolete("Use ColorUtilities class instead. 9/27/2014")]
    public class ColorUtility : ColorUtilities
    {
    }
}
