using System.Windows;

#if WINRT
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#else
using System.Windows.Controls;
using System.Windows.Media;
#endif

namespace MyToolkit.Controls
{
	public class WatermarkedTextBox : Control
	{
		private TextBox textBox;

		public WatermarkedTextBox()
		{
			DefaultStyleKey = typeof(WatermarkedTextBox);
#if WINRT
			GotFocus += delegate { if (textBox != null) textBox.Focus(FocusState.Programmatic); };
#else
			GotFocus += delegate { if (textBox != null) textBox.Focus(); };
#endif
		}

#if WINRT
		protected override void OnApplyTemplate()
		{
			WatermarkBrush = (Brush)Resources["ListViewItemPlaceholderBackgroundThemeBrush"];
#elif WP7
		public override void OnApplyTemplate()
		{
			WatermarkBrush = (Brush)Resources["PhoneSubtleColor"];
#else
		public override void OnApplyTemplate()
		{
			WatermarkBrush = new SolidColorBrush(Colors.DarkGray);
#endif

			base.OnApplyTemplate();
			textBox = (TextBox)GetTemplateChild("textBox");
			textBox.GotFocus += TextBoxOnGotFocus;
			textBox.LostFocus += TextBoxOnLostFocus;
			textBox.TextChanged += TextBoxOnTextChanged;

			originalForeground = textBox.Foreground;

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

		private Brush originalForeground; 
		private void UpdateText()
		{
			checkTextChanges = false; 

			if (textBox == null)
				return; 

			if (string.IsNullOrEmpty(Text) && !hasFocus)
			{
				textBox.Text = Watermark;
				textBox.Foreground = WatermarkBrush;
			}
			else
			{
				textBox.Text = Text ?? "";
				textBox.Foreground = originalForeground;
			}

			checkTextChanges = true;
		}

		#region Dependency Properties

		public static readonly DependencyProperty WatermarkColorProperty =
			DependencyProperty.Register("WatermarkBrush", typeof(Brush), typeof(WatermarkedTextBox), new PropertyMetadata(default(Brush), OnPropertyChanged));

		public Brush WatermarkBrush
		{
			get { return (Brush) GetValue(WatermarkColorProperty); }
			set { SetValue(WatermarkColorProperty, value); }
		}

		public static readonly DependencyProperty WatermarkProperty =
			DependencyProperty.Register("Watermark", typeof(string), typeof(WatermarkedTextBox), new PropertyMetadata("", OnPropertyChanged));

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
