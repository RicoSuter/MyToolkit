using System;
using System.Globalization;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

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

		public static Color FromHex(string colorcode)
		{
			var argb = UInt32.Parse(colorcode.Replace("#", ""), NumberStyles.HexNumber);
			return FromHex(argb);
		}

		public static Color FromHex(uint argb)
		{
			return Color.FromArgb((byte)((argb & -16777216) >> 0x18),
							  (byte)((argb & 0xff0000) >> 0x10),
							  (byte)((argb & 0xff00) >> 8),
							  (byte)(argb & 0xff));
		}
	}
}
