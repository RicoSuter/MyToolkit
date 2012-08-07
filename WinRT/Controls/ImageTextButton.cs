using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace MyToolkit.Controls
{
	[ContentProperty(Name = "Content")]
	public sealed class ImageTextButton : Control
	{
		public ImageTextButton()
		{
			DefaultStyleKey = typeof(ImageTextButton);
		}

		public static readonly DependencyProperty CommandParameterProperty =
			DependencyProperty.Register("CommandParameter", typeof (object), typeof (ImageTextButton), new PropertyMetadata(default(object)));

		public object CommandParameter
		{
			get { return (object) GetValue(CommandParameterProperty); }
			set { SetValue(CommandParameterProperty, value); }
		}

		public static readonly DependencyProperty CommandProperty =
			DependencyProperty.Register("Command", typeof(ICommand), typeof(ImageTextButton), new PropertyMetadata(default(ICommand)));

		public ICommand Command
		{
			get { return (ICommand)GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); }
		}

		public event RoutedEventHandler Click;

		public static readonly DependencyProperty HeaderProperty =
			DependencyProperty.Register("Header", typeof (string), typeof (ImageTextButton), new PropertyMetadata(default(string)));

		public string Header
		{
			get { return (string) GetValue(HeaderProperty); }
			set { SetValue(HeaderProperty, value); }
		}

		public static readonly DependencyProperty ContentProperty =
			DependencyProperty.Register("Content", typeof (FrameworkElement), typeof (ImageTextButton), new PropertyMetadata(default(FrameworkElement)));

		public FrameworkElement Content
		{
			get { return (FrameworkElement) GetValue(ContentProperty); }
			set { SetValue(ContentProperty, value); }
		}	

		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			var presenter = (ContentPresenter)GetTemplateChild("presenter");
			presenter.Content = Content;
		}

		protected override void OnTapped(TappedRoutedEventArgs e)
		{
			base.OnTapped(e);

			var copy = Click;
			if (copy != null)
				copy(this, new RoutedEventArgs());

			if (Command != null && Command.CanExecute(CommandParameter))
				Command.Execute(CommandParameter);
		}
	}
}
