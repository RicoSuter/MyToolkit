using System;
using System.Windows;

#if WP7 || WP8
using MyToolkit.Environment;
using MyToolkit.Paging;

#endif

namespace MyToolkit.Messaging
{
	public static class DefaultActions
	{
#if WP7 || WP8
		public static void GoBack(GoBackMessage message)
		{
			var page = PhonePage.CurrentPage; 
			Deployment.Current.Dispatcher.BeginInvoke(() =>
			{
				try
				{
					page.NavigationService.GoBack();
					if (message.Completed != null)
						message.Completed(true);
				}
				catch
				{
					if (message.Completed != null)
						message.Completed(false);
				}
			});
		}
#endif

#if !WINRT
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
#endif
	}
}
