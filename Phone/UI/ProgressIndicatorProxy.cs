//http://www.codeproject.com/Articles/246355/Binding-the-WP7-ProgressIndicator-in-XAML?display=Print

/*

 <mytoolkit:ProgressIndicatorProxy IsIndeterminate="True" IsVisible="{Binding IsLoading}" />
 
 <Grid x:Name="LayoutRoot" Background="Transparent">
	 <Grid.RowDefinitions>...</Grid.RowDefinitions>
    <mytoolkit:ProgressIndicatorProxy IsIndeterminate="{Binding Indeterminate}" Text="{Binding Message}" Value="{Binding Progress}" />
</Grid>
 
 */

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using MyToolkit.Messaging;
using MyToolkit.Paging;

namespace MyToolkit.UI
{
	public class ProgressIndicatorProxy : FrameworkElement
	{
		public ProgressIndicatorProxy() 
		{
			Loaded += OnLoaded;

		}
	
		bool loaded;
		void OnLoaded(object sender, RoutedEventArgs e)
		{
			if (loaded) 
				return;
			
			Attach();
			loaded = true;
		}

		private ProgressIndicator progressIndicator;
		public void Attach()
		{
			if (DesignerProperties.IsInDesignTool)
				return;

			progressIndicator = SystemTray.ProgressIndicator ?? new ProgressIndicator();
			progressIndicator.Text = Text;
			progressIndicator.IsVisible = IsVisible;
			progressIndicator.IsIndeterminate = IsIndeterminate;
			progressIndicator.Value = Value; 
			SystemTray.SetProgressIndicator(PhonePage.CurrentPage, progressIndicator);
		}

		#region Dependency Properties

		public static readonly DependencyProperty IsIndeterminateProperty = DependencyProperty.RegisterAttached("IsIndeterminate",
			typeof(bool), typeof(ProgressIndicatorProxy), new PropertyMetadata(false, OnIsIndeterminateChanged));

		private static void OnIsIndeterminateChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			var ctrl = (ProgressIndicatorProxy)obj;
			if (ctrl.progressIndicator != null)
			{
				ctrl.progressIndicator.IsIndeterminate = ctrl.IsIndeterminate;
				//SystemTray.SetProgressIndicator(PhonePage.CurrentPage, ctrl.progressIndicator);
			}
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
			{
				ctrl.progressIndicator.IsVisible = ctrl.IsVisible;
				//SystemTray.SetProgressIndicator(PhonePage.CurrentPage, ctrl.progressIndicator);
			}
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
			{
				ctrl.progressIndicator.Text = ctrl.Text;
				//SystemTray.SetProgressIndicator(PhonePage.CurrentPage, ctrl.progressIndicator);
			}
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
			{
				ctrl.progressIndicator.Value = ctrl.Value;
				//SystemTray.SetProgressIndicator(PhonePage.CurrentPage, ctrl.progressIndicator);
			}
		}

		public double Value
		{
			get { return (double)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}

		#endregion
	}
}