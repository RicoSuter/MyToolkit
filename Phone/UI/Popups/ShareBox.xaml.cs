using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Phone.Tasks;

namespace MyToolkit.UI.Popups
{
	public partial class ShareBox : UserControl, IPopupControl
	{
		public static void Show(String title, String text, String textWithoutTitle)
		{
			var control = new ShareBox();
			control.Title = title;
			control.Text = text;
			control.TextWithoutTitle = textWithoutTitle ?? title + ": " + text;
			Popup.Show(control);
		}

		public string Title { get; set; }
		public string Text { get; set; }
		public string TextWithoutTitle { get; set; }

		public ShareBox()
		{
			InitializeComponent();
		}

		public event Action<object> Closed;
		public void SetBackgroundColor(Color color)
		{
			popup.Background = new SolidColorBrush(color);
		}

		public void GoBack()
		{
			Closed(this);
		}

		private void AsMessageClick(object sender, RoutedEventArgs e)
		{
			Closed(this);
			var task = new SmsComposeTask { Body = TextWithoutTitle };
			task.Show();
		}

		private void AsEmailClick(object sender, RoutedEventArgs e)
		{
			Closed(this);
			var task = new EmailComposeTask
			{
				Subject = Title,
				Body = Text
			};
			task.Show();
		}
	}
}
