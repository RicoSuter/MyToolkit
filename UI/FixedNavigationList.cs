using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Phone.Controls;

namespace MyToolkit.UI
{
	public class FixedNavigationList : Control
	{
		public event EventHandler<NavigationListEventArgs> Navigation;

		static FixedNavigationList()
		{
			if (!TiltEffect.TiltableItems.Contains(typeof(ContentPresenter)))
				TiltEffect.TiltableItems.Add(typeof(ContentPresenter));
		}

		public FixedNavigationList()
		{
			DefaultStyleKey = typeof(FixedNavigationList);
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			var itemsControl = (ExtendedItemsControl) GetTemplateChild("itemsControl");
			itemsControl.PrepareContainerForItem += PrepareContainerForItem;
		}

		private void PrepareContainerForItem(object sender, PrepareContainerForItemEventArgs e)
		{
			var element = (UIElement)e.Element;

			element.MouseLeftButtonUp += ElementMouseLeftButtonUp;
			element.ManipulationStarted += ElementManipulationStarted;
			element.ManipulationDelta += ElementManipulationDelta;
		}
	
		#region Properties

		public IEnumerable ItemsSource
		{
			get { return (IEnumerable)GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); }
		}

		public static readonly DependencyProperty ItemsSourceProperty =
			DependencyProperty.Register("ItemsSource", typeof(IEnumerable),
			                            typeof(FixedNavigationList), new PropertyMetadata(null));

		public DataTemplate ItemTemplate
		{
			get { return (DataTemplate)GetValue(ItemTemplateProperty); }
			set { SetValue(ItemTemplateProperty, value); }
		}

		public static readonly DependencyProperty ItemTemplateProperty =
			DependencyProperty.Register("ItemTemplate", typeof(DataTemplate),
			                            typeof(FixedNavigationList), new PropertyMetadata(null));

		#endregion

		#region Events

		private bool manipulationDeltaStarted;
		private void ElementManipulationDelta(object sender, ManipulationDeltaEventArgs e)
		{
			manipulationDeltaStarted = true;
		}

		private void ElementManipulationStarted(object sender, ManipulationStartedEventArgs e)
		{
			manipulationDeltaStarted = false;
		}

		private void ElementMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (manipulationDeltaStarted)
				return;

			var element = (FrameworkElement) sender;
			var child = VisualTreeHelper.GetChild(element, 0);
			if (child != null)
			{
				var menu = ContextMenuService.GetContextMenu(child);
				if (menu != null && menu.IsOpen)
					return;
			}

			OnNavigation(new NavigationListEventArgs(element.DataContext));
		}

		protected void OnNavigation(NavigationListEventArgs args)
		{
			if (Navigation != null)
				Navigation(this, args);
		}

		#endregion
	}
}