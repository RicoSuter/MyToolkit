using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MyToolkit.Controls
{
	public class WatermarkedTextBox : Control
	{
		public WatermarkedTextBox()
		{
			DefaultStyleKey = typeof(WatermarkedTextBox);
		}

		private TextBox textBox;
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			textBox = (TextBox)GetTemplateChild("textBox");
			textBox.GotFocus += TextBoxOnGotFocus;
			textBox.LostFocus += TextBoxOnLostFocus;
			textBox.TextChanged += TextBoxOnTextChanged;

			UpdateText();
		}

		private bool checkTextChanges = true; 
		private void TextBoxOnTextChanged(object sender, TextChangedEventArgs textChangedEventArgs)
		{
			if (!checkTextChanges || !hasFocus)
				return;

			Text = textBox.Text; 
			UpdateText();
		}

		private bool hasFocus = false; 
		private void TextBoxOnLostFocus(object sender, RoutedEventArgs routedEventArgs)
		{
			hasFocus = false; 
			UpdateText();
		}

		private void TextBoxOnGotFocus(object sender, RoutedEventArgs routedEventArgs)
		{
			hasFocus = true; 
			UpdateText();
		}

		private void UpdateText()
		{
			checkTextChanges = false; 

			if (textBox == null)
				return; 

			if (string.IsNullOrEmpty(Text) && !hasFocus)
			{
				textBox.Text = Watermark;
				textBox.Foreground = new SolidColorBrush(Colors.DarkGray);
			}
			else
			{
				textBox.Text = Text;
				textBox.Foreground = new SolidColorBrush(Colors.Black);
			}

			checkTextChanges = true;
		}

		#region Dependency Properties

		public static readonly DependencyProperty WatermarkProperty =
			DependencyProperty.Register("Watermark", typeof(string), typeof(WatermarkedTextBox), new PropertyMetadata(default(string), OnPropertyChanged));

		public string Watermark
		{
			get { return (string) GetValue(WatermarkProperty); }
			set { SetValue(WatermarkProperty, value); }
		}

		public static readonly DependencyProperty TextProperty =
			DependencyProperty.Register("Text", typeof(string), typeof(WatermarkedTextBox), new PropertyMetadata("", OnPropertyChanged));

		public string Text
		{
			get { return (string) GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}

		private static void OnPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			var ctrl = (WatermarkedTextBox)obj;
			ctrl.UpdateText();
		}

		#endregion
	}
}
