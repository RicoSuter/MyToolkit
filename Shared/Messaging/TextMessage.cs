using System;
using System.Windows;

#if WINRT
using System.Threading.Tasks;
using MyToolkit.Resources;
using Windows.UI.Popups;
#endif

namespace MyToolkit.Messaging
{
	public class TextMessage
	{
		public enum MessageButton
		{
			OK, 
			OKCancel, 
			YesNoCancel, 
			YesNo
		}

		public enum MessageResult
		{
			OK,
			Cancel, 
			Yes, 
			No
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

#if WINRT
		public static Func<TextMessage, Task> GetAction()
		{
			return ShowTextMessage;
		}

		private static async Task ShowTextMessage(TextMessage m)
		{
			if (m.Button == MessageButton.OK)
			{
				var msg = new MessageDialog(m.Text, m.Title);
				await msg.ShowAsync();
			}
			else
			{
				var msg = new MessageDialog(m.Text, m.Title);
				
				if (m.Button == MessageButton.OKCancel)
				{
					msg.Commands.Add(new UICommand(Strings.ButtonOk));
					msg.Commands.Add(new UICommand(Strings.ButtonCancel));
				}
				else if (m.Button == MessageButton.YesNoCancel || m.Button == MessageButton.YesNo)
				{
					msg.Commands.Add(new UICommand(Strings.ButtonYes));
					msg.Commands.Add(new UICommand(Strings.ButtonNo));
					
					if (m.Button == MessageButton.YesNoCancel)
						msg.Commands.Add(new UICommand(Strings.ButtonCancel));
				}

				msg.DefaultCommandIndex = 0;
				msg.CancelCommandIndex = 1;

				var cmd = await msg.ShowAsync();

				var index = msg.Commands.IndexOf(cmd); 
				if (m.Button == MessageButton.OKCancel)
					m.Result = index == 0 ? MessageResult.OK : MessageResult.Cancel;
				else if (m.Button == MessageButton.YesNoCancel)
					m.Result = index == 0 ? MessageResult.Yes : (index == 1 ? MessageResult.No : MessageResult.Cancel);
				else if (m.Button == MessageButton.YesNo)
					m.Result = index == 0 ? MessageResult.Yes : MessageResult.No;
			}
		}

#else
#if !WP7
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
