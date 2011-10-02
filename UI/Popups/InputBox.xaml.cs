using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MyToolkit.UI.Popups
{
	public partial class InputBox : UserControl, IPopupControl
	{
		public static void Show(String message, String title, bool showCancel, Action<object, string> completed)
		{
			var control = new InputBox();
			control.title.Text = title;
			control.message.Text = message;
			control.completed = completed;
			control.cancel.Visibility = showCancel ? 
				Visibility.Visible : Visibility.Collapsed; 

			Popup.Show(control);
		}

		public InputBox()
		{
			InitializeComponent();
			Loaded += delegate { box.Focus(); };
		}

		private Action<object, string> completed;
		public event Action<object> Closed;
		public void SetBackgroundColor(Color color)
		{
			popup.Background = new SolidColorBrush(color);
		}

		public void GoBack()
		{
			if (cancel.Visibility == Visibility.Visible)
				Cancel(null, null);
			else
				Close(null, null);
		}

		private void Cancel(object sender, System.Windows.RoutedEventArgs e)
		{
			Closed(this);
			completed(this, null);
		}

		private void Close(object sender, System.Windows.RoutedEventArgs e)
		{
			Closed(this);
			completed(this, box.Text);
		}
	}
}
