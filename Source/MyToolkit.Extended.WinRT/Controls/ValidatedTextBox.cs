using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using MyToolkit.MVVM;
using MyToolkit.Validation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyToolkit.Controls
{
	public sealed class ValidatedTextBox : Control, IValidationControl
	{
		public static readonly DependencyProperty ErrorTextProperty =
			DependencyProperty.Register("ErrorText", typeof(string), typeof(ValidatedTextBox), new PropertyMetadata(default(string)));

		public string ErrorText
		{
			get { return (string)GetValue(ErrorTextProperty); }
			set { SetValue(ErrorTextProperty, value); }
		}

		public static readonly DependencyProperty HeaderProperty =
			DependencyProperty.Register("Header", typeof (string), typeof (ValidatedTextBox), new PropertyMetadata(default(string)));

		public string Header
		{
			get { return (string) GetValue(HeaderProperty); }
			set { SetValue(HeaderProperty, value); }
		}

		public static readonly DependencyProperty ValidationExceptionProperty =
			DependencyProperty.Register("ValidationException", typeof (Exception), typeof (ValidatedTextBox), new PropertyMetadata(default(Exception)));

		public Exception ValidationException
		{
			get { return (Exception) GetValue(ValidationExceptionProperty); }
			set { SetValue(ValidationExceptionProperty, value); }
		}
		
		public static readonly DependencyProperty TextProperty =
			DependencyProperty.Register("Text", typeof (string), typeof (ValidatedTextBox),
			new PropertyMetadata(default(string), DependencyPropertyHelper.CallMethod<ValidatedTextBox>(m => m.Validate())));

		public string Text
		{
			get { return (string) GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}

		public static readonly DependencyProperty MinLengthProperty =
			DependencyProperty.Register("MinLength", typeof (int), typeof (ValidatedTextBox), new PropertyMetadata(default(int)));

		public int MinLength
		{
			get { return (int) GetValue(MinLengthProperty); }
			set { SetValue(MinLengthProperty, value); }
		}

		public static readonly DependencyProperty MaxLengthProperty =
			DependencyProperty.Register("MaxLength", typeof (int), typeof (ValidatedTextBox), 
			new PropertyMetadata(default(int), DependencyPropertyHelper.CallMethod<ValidatedTextBox>(m => m.Validate())));

		public int MaxLength
		{
			get { return (int) GetValue(MaxLengthProperty); }
			set { SetValue(MaxLengthProperty, value); }
		}

		public static readonly DependencyProperty ValidationRegularExpressionProperty =
			DependencyProperty.Register("ValidationRegularExpression", typeof (string), 
			typeof (ValidatedTextBox), new PropertyMetadata(default(string), DependencyPropertyHelper.CallMethod<ValidatedTextBox>(m => m.OnValidationRegularExpressionChanged())));

		public string ValidationRegularExpression
		{
			get { return (string) GetValue(ValidationRegularExpressionProperty); }
			set { SetValue(ValidationRegularExpressionProperty, value); }
		}

		private Regex regex = null; 
		private void OnValidationRegularExpressionChanged()
		{
			if (!string.IsNullOrEmpty(ValidationRegularExpression))
				regex = new Regex(ValidationRegularExpression);
			else
				regex = null; 
		}

		private void Validate()
		{
			ValidationException = null;

			if (Text == null)
				Text = "";

			if (MaxLength > 0 && Text.Length > MaxLength)
				ValidationException = new ValidationException(string.IsNullOrEmpty(ErrorText) ? "Too long" : ErrorText);
			if (MinLength > 0 && Text.Length < MinLength)
				ValidationException = new ValidationException(string.IsNullOrEmpty(ErrorText) ? "Too short" : ErrorText);
			else if (regex != null && !regex.IsMatch(Text))
				ValidationException = new ValidationException(string.IsNullOrEmpty(ErrorText) ? "Text does not match" : ErrorText);

			var copy = ValidationChanged;
			if (copy != null)
				copy(this, ValidationException);
		}

		public event EventHandler<Exception> ValidationChanged;

		public ValidatedTextBox()
		{
			DefaultStyleKey = typeof(ValidatedTextBox);
			Loaded += delegate
			{
				ValidationContainer.Register(this);
				Validate();
			};
		}
	}
}
