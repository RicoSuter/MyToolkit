using System.Windows;

namespace MyToolkit.Messages
{
	public static partial class DefaultActions
	{
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
