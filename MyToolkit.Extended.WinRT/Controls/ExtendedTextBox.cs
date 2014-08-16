using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyToolkit.Controls
{
	public class ExtendedTextBox : TextBox
	{
		public ExtendedTextBox()
		{
			TextChanged += OnTextChanged;
		}

		private void OnTextChanged(object sender, TextChangedEventArgs textChangedEventArgs)
		{
			if (ImmediateUpdates)
				Text = base.Text;
		}

		public static readonly DependencyProperty TextProperty =
			DependencyProperty.Register("Text", typeof (string), typeof (ExtendedTextBox), 
			new PropertyMetadata(default(string), (o, args) => ((ExtendedTextBox)o).SetText(args.NewValue)));

		private void SetText(object newValue)
		{
			base.Text = newValue != null ? (String)newValue : "";
		}

		public string Text
		{
			get { return (string) GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}	
		
		public static readonly DependencyProperty ImmediateUpdatesProperty =
			DependencyProperty.Register("ImmediateUpdates", typeof (bool), typeof (ExtendedTextBox), new PropertyMetadata(true));

		public bool ImmediateUpdates
		{
			get { return (bool) GetValue(ImmediateUpdatesProperty); }
			set { SetValue(ImmediateUpdatesProperty, value); }
		}
	}
}