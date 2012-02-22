using System.Windows;
using Microsoft.Phone.Controls;

namespace MyToolkit.Messages
{
	public static partial class DefaultActions
	{
		public static void GoBack(GoBackMessage message, PhoneApplicationPage page)
		{
			Deployment.Current.Dispatcher.BeginInvoke(() => page.NavigationService.GoBack());
		}
	}
}
