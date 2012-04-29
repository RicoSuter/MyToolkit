using System;
using System.Diagnostics;
using System.Windows;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Info;

namespace MyToolkit.Environment
{
	public static class PhoneApplication
	{
		public static PhoneApplicationPage CurrentPage
		{
			get { return (PhoneApplicationPage) ((PhoneApplicationFrame)Application.Current.RootVisual).Content; }
		}

		public static bool IsDarkTheme
		{
			get { return Resources.PhoneLightThemeVisibility == Visibility.Collapsed; }
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
