using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MyToolkit.Utilities;

namespace MyToolkit.UI.Popups
{
	public partial class InputBox
	{
		#region Static methods

		public static Task<string> ShowAsync(String message, String title, string input, bool showCancel)
		{
			return TaskHelper.RunCallbackMethod<string, string, string, bool, string>(Show, message, title, input, showCancel);
		}

		public static Task<string> ShowAsync(String message, String title, bool showCancel)
		{
			return TaskHelper.RunCallbackMethod<string, string, string, bool, string>(Show, message, title, string.Empty, showCancel);
		}

		public static void Show(String message, String title, string input, bool showCancel, Action<string> completed)
		{
			Show(message, title, input, showCancel, (o, s) => completed(s));
		}

		public static void Show(String message, String title, string input, bool showCancel, Action<object, string> completed)
		{
			var control = new InputBox();
			control.title.Text = title;
			control.message.Text = message;
			control.box.Text = input;
			control.box.SelectionStart = input.Length;
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
