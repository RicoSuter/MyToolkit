//http://www.codeproject.com/Articles/246355/Binding-the-WP7-ProgressIndicator-in-XAML?display=Print

/*
 
 <Grid x:Name="LayoutRoot" Background="Transparent">
	 <Grid.RowDefinitions>...</Grid.RowDefinitions>
    <mytoolkit:ProgressIndicatorProxy IsIndeterminate="{Binding Indeterminate}" Text="{Binding Message}" Value="{Binding Progress}" />
</Grid>
 
 */

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace MyToolkit.UI
{
	public class ProgressIndicatorProxy : FrameworkElement
	{
		bool loaded;

		public ProgressIndicatorProxy() 
		{
			Loaded += OnLoaded;
		}

		void OnLoaded(object sender, RoutedEventArgs e)
		{
			if (loaded)
			{
				return;
			}

			Attach();

			loaded = true;
		}

		public void Attach()
		{
			if (DesignerProperties.IsInDesignTool)
			{
				return;
			}

			var page = (PhoneApplicationPage)((PhoneApplicationFrame)Application.Current.RootVisual).Content; //this.Parent.GetVisualAncestors<PhoneApplicationPage>().First();

			var progressIndicator = SystemTray.ProgressIndicator;
			if (progressIndicator != null)
			{
				return;
			}

			progressIndicator = new ProgressIndicator();

			SystemTray.SetProgressIndicator(page, progressIndicator);

			Binding binding = new Binding("IsIndeterminate") { Source = this };
			BindingOperations.SetBinding(
				progressIndicator, ProgressIndicator.IsIndeterminateProperty, binding);

			binding = new Binding("IsVisible") { Source = this };
			BindingOperations.SetBinding(
				progressIndicator, ProgressIndicator.IsVisibleProperty, binding);

			binding = new Binding("Text") { Source = this };
			BindingOperations.SetBinding(
				progressIndicator, ProgressIndicator.TextProperty, binding);

			binding = new Binding("Value") { Source = this };
			BindingOperations.SetBinding(
				progressIndicator, ProgressIndicator.ValueProperty, binding);
		}

		#region IsIndeterminate

		public static readonly DependencyProperty IsIndeterminateProperty
			= DependencyProperty.RegisterAttached(
				"IsIndeterminate",
				typeof(bool),
				typeof(ProgressIndicatorProxy), new PropertyMetadata(false));

		public bool IsIndeterminate
		{
			get
			{
				return (bool)GetValue(IsIndeterminateProperty);
			}
			set
			{
				SetValue(IsIndeterminateProperty, value);
			}
		}

		#endregion

		#region IsVisible

		public static readonly DependencyProperty IsVisibleProperty
			= DependencyProperty.RegisterAttached(
				"IsVisible",
				typeof(bool),
				typeof(ProgressIndicatorProxy), new PropertyMetadata(true));

		public bool IsVisible
		{
			get
			{
				return (bool)GetValue(IsVisibleProperty);
			}
			set
			{
				SetValue(IsVisibleProperty, value);
			}
		}

		#endregion

		#region Text

		public static readonly DependencyProperty TextProperty
			= DependencyProperty.RegisterAttached(
				"Text",
				typeof(string),
				typeof(ProgressIndicatorProxy), new PropertyMetadata(string.Empty));

		public string Text
		{
			get
			{
				return (string)GetValue(TextProperty);
			}
			set
			{
				SetValue(TextProperty, value);
			}
		}

		#endregion

		#region Value

		public static readonly DependencyProperty ValueProperty
			= DependencyProperty.RegisterAttached(
				"Value",
				typeof(double),
				typeof(ProgressIndicatorProxy), new PropertyMetadata(0.0));

		public double Value
		{
			get
			{
				return (double)GetValue(ValueProperty);
			}
			set
			{
				SetValue(ValueProperty, value);
			}
		}

		#endregion
	}
}
