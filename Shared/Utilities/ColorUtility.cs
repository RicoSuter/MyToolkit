using System;
using System.Globalization;
#if !WINRT
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
#else
using System.Reflection;
using Windows.UI;
#endif

namespace MyToolkit.Utilities
{
	public static class ColorUtility
	{
		public static Color Mix(Color foreground, double alpha, Color background)
		{
			var diff = 1.0 - alpha;
			var color = Color.FromArgb(foreground.A,
				(byte)(foreground.R * alpha + background.R * diff),
				(byte)(foreground.G * alpha + background.G * diff),
				(byte)(foreground.B * alpha + background.B * diff));
			return color; 
		}

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

		public static Color ChangeAlpha(Color color, byte alpha)
		{
			return Color.FromArgb(alpha, color.R, color.G, color.B);
		}

		public static Color ChangeAlpha(Color color, string alpha)
		{
			var value = UInt32.Parse(alpha, NumberStyles.HexNumber);
			return ChangeAlpha(color, (byte)(value & 0xff));
		}

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

		public static Color FromHex(string colorCode)
		{
			colorCode = colorCode.Replace("#", "");
			if (colorCode.Length == 6)
				colorCode = "FF" + colorCode;
			return FromHex(UInt32.Parse(colorCode, NumberStyles.HexNumber));
		}

		public static Color FromHex(uint argb)
		{
			return Color.FromArgb((byte)((argb & -16777216) >> 0x18),
							  (byte)((argb & 0xff0000) >> 0x10),
							  (byte)((argb & 0xff00) >> 8),
							  (byte)(argb & 0xff));
		}

#if WINRT

		public static Color FromString(string value)
		{
			var property = typeof(Colors).GetTypeInfo().GetProperty(value);
			if (property != null)
				return (Color)property.GetValue(null);
			return ColorUtility.FromHex(value);
		}

#endif
	}
}
