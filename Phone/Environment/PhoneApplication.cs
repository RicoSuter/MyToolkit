using System;
using System.Diagnostics;
using System.Windows;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Info;

namespace MyToolkit.Environment
{
	public static class PhoneApplication
	{
		public static bool IsDarkTheme
		{
			get { return Resources.PhoneLightThemeVisibility == Visibility.Collapsed; }
		}

		private static readonly Version _targetedVersion8 = new Version(8, 0);
		private static readonly Version _targetedVersion78 = new Version(7, 10, 8858);

		public static bool IsWindowsPhone7
		{
			get { return System.Environment.OSVersion.Version < _targetedVersion8; }
		}

		public static bool IsWindowsPhone78
		{
			get { return System.Environment.OSVersion.Version >= _targetedVersion78 || !IsWindowsPhone8; }
		}

		public static bool IsWindowsPhone8
		{
			get { return System.Environment.OSVersion.Version >= _targetedVersion8; }
		}

		public static bool IsLowMemoryDevice
		{
			get
			{
				try
				{
					var result = (long)DeviceExtendedProperties.GetValue("ApplicationWorkingSetLimit");
					return result < 94371840; 

				}
				catch (ArgumentOutOfRangeException)
				{
					return false; 
				}
			}
		}
	}
}
