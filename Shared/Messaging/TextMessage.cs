using System;
using System.Windows;

#if METRO
using System.Threading.Tasks;
using Windows.UI.Popups;
#endif

namespace MyToolkit.Messaging
{
	public class TextMessage
	{
		public enum MessageButton
		{
			OK, 
			OKCancel
		}

		public enum MessageResult
		{
			OK,
			Cancel
		}

		public TextMessage(string text)
			: this(text, "", MessageButton.OK) { }

		public TextMessage(string text, string title)
			: this(text, title, MessageButton.OK) { }

		public TextMessage(string text, string title, MessageButton button)
		{
			Text = text;
			Title = title;
			Button = button;
			Result = MessageResult.OK;
		}

		public string Title { get; set; }
		public string Text { get; set; }
		public MessageButton Button { get; set; }

		public MessageResult Result { get; set; }

#if METRO
		public static Func<TextMessage, Task> GetAction()
		{
			return ShowTextMessage;
		}

		private static async Task ShowTextMessage(TextMessage m)
		{
			if (m.Button == MessageButton.OK)
			{
				var msg = new MessageDialog(m.Text, m.Title);
				msg.ShowAsync();
			}
			else
			{
				var msg = new MessageDialog(m.Text, m.Title);
				msg.Commands.Add(new UICommand("OK"));
				msg.Commands.Add(new UICommand("Cancel"));
				msg.DefaultCommandIndex = 0;
				msg.CancelCommandIndex = 1;

				var cmd = await msg.ShowAsync();
				m.Result = msg.Commands.IndexOf(cmd) == 0 ? MessageResult.OK : MessageResult.Cancel;
			}
		}

#else
#if !WINDOWS_PHONE
		public static Action<TextMessage> GetAction()
		{
			return message =>
			{
				if (message.Button == MessageButton.OK)
					MessageBox.Show(message.Text, message.Title, MessageBoxButton.OK);
				else if (message.Button == MessageButton.OKCancel)
				{
					var result = MessageBox.Show(message.Text, message.Title, MessageBoxButton.OKCancel);
					message.Result = result == MessageBoxResult.OK ? MessageResult.OK : MessageResult.Cancel;
				}
			};
		}
#endif
#endif
	}
}
