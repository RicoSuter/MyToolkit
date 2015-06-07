using MyToolkit.UI;
using Windows.UI.ApplicationSettings;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyToolkit.Controls
{
	public sealed class SettingsHeader : Control
	{
		public SettingsHeader()
		{
			DefaultStyleKey = typeof(SettingsHeader);
		}

		public static readonly DependencyProperty HeaderProperty =
			DependencyProperty.Register("Header", typeof (string), typeof (SettingsHeader), new PropertyMetadata(default(string)));

		public string Header
		{
			get { return (string) GetValue(HeaderProperty); }
			set { SetValue(HeaderProperty, value); }
		}

		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			var button = (Button) GetTemplateChild("button");
			button.Click += OnBackClicked;
		}

		private void OnBackClicked(object sender, RoutedEventArgs e)
		{
			var parent = PopupHelper.GetParentPopup(this);
			if (parent != null)
				parent.IsOpen = false;

			if (ApplicationView.Value != ApplicationViewState.Snapped)
				SettingsPane.Show();
		}
	}
}
