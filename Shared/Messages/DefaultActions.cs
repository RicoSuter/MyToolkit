using System.Windows;

#if WINDOWS_PHONE
using Microsoft.Phone.Controls;
#endif

namespace MyToolkit.Messages
{
	public static class DefaultActions
	{
#if WINDOWS_PHONE
		public static void GoBack(GoBackMessage message, PhoneApplicationPage page)
		{
			Deployment.Current.Dispatcher.BeginInvoke(() => page.NavigationService.GoBack());
		}
#endif

		public static void ShowTextMessage(TextMessage message)
		{
			if (message.Button == TextMessage.MessageButton.OK)
				MessageBox.Show(message.Text, message.Title, MessageBoxButton.OK);
			else if(message.Button == TextMessage.MessageButton.OKCancel)
			{
				var result = MessageBox.Show(message.Text, message.Title, MessageBoxButton.OKCancel);
				message.Result = result == MessageBoxResult.OK ? TextMessage.MessageResult.OK : TextMessage.MessageResult.Cancel;
			}
		}
	}
}
