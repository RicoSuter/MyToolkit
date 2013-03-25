using System;
using System.Windows;
using System.Windows.Input;

namespace MyToolkit.UI.Popups
{
	public partial class InputBox
	{
		#region Static methods

		public static void Show(String message, String title, string input, bool showCancel, Action<object, string> completed)
		{
			var control = new InputBox();
			control.title.Text = title;
			control.message.Text = message;
			control.box.Text = input;
			control.cancel.Visibility = showCancel ?
				Visibility.Visible : Visibility.Collapsed;

			Show(control, false, false, delegate { completed(control, control.Text); });
		}

		public static void Show(String message, String title, bool showCancel, Action<object, string> completed)
		{
			Show(message, title, string.Empty, showCancel, completed);
		}

		#endregion

		public string Text { get; set; }

		internal InputBox()
		{
			InitializeComponent();
			Loaded += delegate { box.Focus(); };
		}

		public override void GoBack()
		{
			if (cancel.Visibility == Visibility.Visible)
				Cancel(null, null);
		}

		private void Cancel(object sender, RoutedEventArgs e)
		{
			Text = null; 
			Close();
		}

		private void Close(object sender, RoutedEventArgs e)
		{
			Text = box.Text; 
			Close();
		}

		private void Box_OnKeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
				Close(sender, e);
		}
	}
}
