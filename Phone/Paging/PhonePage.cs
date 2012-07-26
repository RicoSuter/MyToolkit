using System.Windows;
using Microsoft.Phone.Controls;

namespace MyToolkit.Paging
{
	public static class PhonePage
	{
		public static PhoneApplicationPage CurrentPage
		{
			get { return (PhoneApplicationPage)((PhoneApplicationFrame)Application.Current.RootVisual).Content; }
		}
	}
}
