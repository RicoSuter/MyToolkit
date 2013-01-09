using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyToolkit.Controls
{
	public class TextButton : Button
	{
		public TextButton()
		{
			DefaultStyleKey = typeof(TextButton);
		}

		public static readonly DependencyProperty ShowTextOnlyInLandscapeProperty =
			DependencyProperty.Register("ShowTextOnlyInLandscape", typeof (bool), typeof (TextButton), new PropertyMetadata(default(bool), PropertyChangedCallback));

		private static void PropertyChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			var btn = (TextButton) obj;
			Window.Current.SizeChanged -= btn.OnSizeChanged;
			if (btn.ShowTextOnlyInLandscape)
				Window.Current.SizeChanged += btn.OnSizeChanged;
		}

		public bool ShowTextOnlyInLandscape
		{
			get { return (bool) GetValue(ShowTextOnlyInLandscapeProperty); }
			set { SetValue(ShowTextOnlyInLandscapeProperty, value); }
		}

		private TextBlock textLabel; 
		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			textLabel = (TextBlock)GetTemplateChild("TextLabel");
			UpdateVisibility();
		}

		private void OnSizeChanged(object sender, WindowSizeChangedEventArgs windowSizeChangedEventArgs)
		{
			UpdateVisibility();
		}

		private void UpdateVisibility()
		{
			if (ShowTextOnlyInLandscape)
				textLabel.Visibility = ApplicationView.Value == ApplicationViewState.FullScreenLandscape
					? Visibility.Visible : Visibility.Collapsed;
		}

		public static readonly DependencyProperty HeaderProperty =
			DependencyProperty.Register("Header", typeof (String), typeof (TextButton), new PropertyMetadata(default(String)));

		public String Header
		{
			get { return (String) GetValue(HeaderProperty); }
			set { SetValue(HeaderProperty, value); }
		}
	}
}
