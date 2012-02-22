using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace MyToolkit.Phone
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
