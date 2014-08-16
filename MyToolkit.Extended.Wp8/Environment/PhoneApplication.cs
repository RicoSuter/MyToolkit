using System;
using System.Windows;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Info;
using Microsoft.Phone.Marketplace;
using MyToolkit.Paging;

namespace MyToolkit.Environment
{
	public static class PhoneApplication
	{
		public static bool IsTrial
		{
			get
			{
#if DEBUG
				return false;
#else
				var license = new LicenseInformation();
				return license.IsTrial();
#endif
			}
		}

		private static bool _isNavigating = false;
		/// <summary>
		/// Gets a value which indicates if the application is navigating to another page. 
		/// This works only if ExtendedPage (or AnimatedPage) is always used!
		/// </summary>
		public static bool IsNavigating
		{
			get
			{
				lock (typeof(PhoneApplicationPageExtensions))
					return _isNavigating;
			}
			set
			{
				lock (typeof(PhoneApplicationPageExtensions))
					_isNavigating = value;
			}
		}

		public static PhoneApplicationPage CurrentPage
		{
			get { return (PhoneApplicationPage)((PhoneApplicationFrame)Application.Current.RootVisual).Content; }
		}

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
