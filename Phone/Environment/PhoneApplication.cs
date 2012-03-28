using System.Windows;
using Microsoft.Phone.Controls;

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
	}
}
