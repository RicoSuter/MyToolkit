using System;
using System.Windows;
using Microsoft.Phone.Tasks;

namespace MyToolkit.UI.Popups
{
	public partial class ShareBox
	{
		#region Static methods

		public static void Show(String title, String text, String textWithoutTitle)
		{
			var control = new ShareBox();
			control.Title = title;
			control.Text = text;
			control.TextWithoutTitle = textWithoutTitle ?? title + ": " + text;
			Show(control);
		}

		#endregion

		public string Title { get; set; }
		public string Text { get; set; }
		public string TextWithoutTitle { get; set; }

		protected ShareBox()
		{
			InitializeComponent();
		}

		public override void GoBack()
		{
			Close();
		}

		private void AsMessageClick(object sender, RoutedEventArgs e)
		{
			Close();
			var task = new SmsComposeTask { Body = TextWithoutTitle };
			task.Show();
		}

		private void AsEmailClick(object sender, RoutedEventArgs e)
		{
			Close();
			var task = new EmailComposeTask
			{
				Subject = Title,
				Body = Text
			};
			task.Show();
		}
	}
}
