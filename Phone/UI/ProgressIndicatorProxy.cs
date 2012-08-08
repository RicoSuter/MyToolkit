/*
<Grid x:Name="LayoutRoot" Background="Transparent">
	<Grid.RowDefinitions>...</Grid.RowDefinitions>
    <mytoolkit:ProgressIndicatorProxy IsIndeterminate="True" IsVisible="{Binding IsLoading}" />
</Grid>
 */

using System.ComponentModel;
using System.Windows;
using Microsoft.Phone.Shell;
using MyToolkit.Paging;

namespace MyToolkit.UI
{
	public class ProgressIndicatorProxy : FrameworkElement
	{
		public ProgressIndicatorProxy() 
		{
			Loaded += OnLoaded;
		}

		private bool loaded;
		private ProgressIndicator progressIndicator;
		void OnLoaded(object sender, RoutedEventArgs e)
		{
			if (loaded || DesignerProperties.IsInDesignTool) 
				return;
			loaded = true;

			progressIndicator = SystemTray.ProgressIndicator;
			if (progressIndicator == null)
			{
				progressIndicator = new ProgressIndicator();
				SystemTray.SetProgressIndicator(PhonePage.CurrentPage, progressIndicator);
			}

			progressIndicator.Text = Text;
			progressIndicator.IsVisible = IsVisible;
			progressIndicator.IsIndeterminate = IsIndeterminate;
			progressIndicator.Value = Value; 
		}

		public static readonly DependencyProperty IsIndeterminateProperty = DependencyProperty.RegisterAttached("IsIndeterminate",
			typeof(bool), typeof(ProgressIndicatorProxy), new PropertyMetadata(false, OnIsIndeterminateChanged));

		private static void OnIsIndeterminateChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			var ctrl = (ProgressIndicatorProxy)obj;
			if (ctrl.progressIndicator != null)
				ctrl.progressIndicator.IsIndeterminate = ctrl.IsIndeterminate;
		}

		public bool IsIndeterminate
		{
			get { return (bool)GetValue(IsIndeterminateProperty); }
			set { SetValue(IsIndeterminateProperty, value); }
		}

		public static readonly DependencyProperty IsVisibleProperty = DependencyProperty.RegisterAttached("IsVisible",
			typeof(bool), typeof(ProgressIndicatorProxy), new PropertyMetadata(true, OnIsVisibleChanged));

		private static void OnIsVisibleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			var ctrl = (ProgressIndicatorProxy)obj;
			if (ctrl.progressIndicator != null)
				ctrl.progressIndicator.IsVisible = ctrl.IsVisible;
		}

		public bool IsVisible
		{
			get { return (bool)GetValue(IsVisibleProperty); }
			set { SetValue(IsVisibleProperty, value); }
		}

		public static readonly DependencyProperty TextProperty = DependencyProperty.RegisterAttached("Text", 
			typeof(string), typeof(ProgressIndicatorProxy), new PropertyMetadata(string.Empty, OnTextChanged));

		private static void OnTextChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			var ctrl = (ProgressIndicatorProxy)obj;
			if (ctrl.progressIndicator != null)
				ctrl.progressIndicator.Text = ctrl.Text;
		}

		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}

		public static readonly DependencyProperty ValueProperty = DependencyProperty.RegisterAttached("Value",
			typeof(double), typeof(ProgressIndicatorProxy), new PropertyMetadata(0.0, OnValueChanged));

		private static void OnValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			var ctrl = (ProgressIndicatorProxy)obj;
			if (ctrl.progressIndicator != null)
				ctrl.progressIndicator.Value = ctrl.Value;
		}

		public double Value
		{
			get { return (double)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}
	}
}