//-----------------------------------------------------------------------
// <copyright file="WatermarkedTextBox.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

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
		private TextBox _textBox;
        private bool _checkTextChanges = true;
        private bool _hasFocus = false; 

		public WatermarkedTextBox()
		{
			DefaultStyleKey = typeof(WatermarkedTextBox);
#if WINRT
			GotFocus += delegate { if (_textBox != null) _textBox.Focus(FocusState.Programmatic); };
#else
			GotFocus += delegate { if (_textBox != null) _textBox.Focus(); };
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
			_textBox = (TextBox)GetTemplateChild("_textBox");
			_textBox.GotFocus += TextBoxOnGotFocus;
			_textBox.LostFocus += TextBoxOnLostFocus;
			_textBox.TextChanged += TextBoxOnTextChanged;

			originalForeground = _textBox.Foreground;

			UpdateText();
		}

		private void TextBoxOnTextChanged(object sender, TextChangedEventArgs textChangedEventArgs)
		{
			if (!_checkTextChanges || !_hasFocus)
				return;

			Text = _textBox.Text; 
			UpdateText();
		}

		private void TextBoxOnLostFocus(object sender, RoutedEventArgs routedEventArgs)
		{
			_hasFocus = false; 
			UpdateText();
		}

		private void TextBoxOnGotFocus(object sender, RoutedEventArgs routedEventArgs)
		{
			_hasFocus = true; 
			UpdateText();
		}

		private Brush originalForeground; 
		private void UpdateText()
		{
			_checkTextChanges = false; 

			if (_textBox == null)
				return; 

			if (string.IsNullOrEmpty(Text) && !_hasFocus)
			{
				_textBox.Text = Watermark;
				_textBox.Foreground = WatermarkBrush;
			}
			else
			{
				_textBox.Text = Text ?? "";
				_textBox.Foreground = originalForeground;
			}

			_checkTextChanges = true;
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
