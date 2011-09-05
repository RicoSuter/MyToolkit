using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections;
using System.Windows.Media;
using Microsoft.Phone.Controls;

// found on http://www.scottlogic.co.uk/blog/colin/2011/04/a-fast-loading-windows-phone-7-navigationlist-control/

namespace MyToolkit.UI
{
	public class NavigationListEventArgs : EventArgs
	{
		internal NavigationListEventArgs(object item)
		{
			Item = item;
		}

		public object Item { private set; get; }
	}

	public class FixedNavigationList : Control
	{
		static FixedNavigationList()
		{
			if (!TiltEffect.TiltableItems.Contains(typeof(ContentPresenter)))
				TiltEffect.TiltableItems.Add(typeof(ContentPresenter));
		}
	
		private bool manipulationDeltaStarted;
	
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

		public FixedNavigationList()
		{
			DefaultStyleKey = typeof(FixedNavigationList);
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			var itemsControl = (ItemsControlEx) GetTemplateChild("itemsControl");
			itemsControl.PrepareContainerForItem += ItemsControl_PrepareContainerForItem;
		}

		private void ItemsControl_PrepareContainerForItem(object sender, PrepareContainerForItemEventArgs e)
		{
			var element = (UIElement) e.Element;

			element.MouseLeftButtonUp += ElementMouseLeftButtonUp;
			element.ManipulationStarted += ElementManipulationStarted;
			element.ManipulationDelta += ElementManipulationDelta;
		}

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

		public event EventHandler<NavigationListEventArgs> Navigation;

		protected void OnNavigation(NavigationListEventArgs args)
		{
			if (Navigation != null)
				Navigation(this, args);
		}
	}

	public class NavigationList : Control
	{
		static NavigationList()
		{
			if (!TiltEffect.TiltableItems.Contains(typeof(ContentPresenter)))
				TiltEffect.TiltableItems.Add(typeof(ContentPresenter));
		}


		private bool manipulationDeltaStarted;
	
		public IEnumerable ItemsSource
		{
			get { return (IEnumerable)GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); }
		}

		public static readonly DependencyProperty ItemsSourceProperty =
			DependencyProperty.Register("ItemsSource", typeof(IEnumerable),
			typeof(NavigationList), new PropertyMetadata(null));

		public DataTemplate ItemTemplate
		{
			get { return (DataTemplate)GetValue(ItemTemplateProperty); }
			set { SetValue(ItemTemplateProperty, value); }
		}

		public static readonly DependencyProperty ItemTemplateProperty =
			DependencyProperty.Register("ItemTemplate", typeof(DataTemplate),
			typeof(NavigationList), new PropertyMetadata(null));

		public NavigationList()
		{
			DefaultStyleKey = typeof(NavigationList);
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			var itemsControl = (ItemsControlEx) GetTemplateChild("itemsControl");
			itemsControl.PrepareContainerForItem += ItemsControl_PrepareContainerForItem;
		}

		private void ItemsControl_PrepareContainerForItem(object sender, PrepareContainerForItemEventArgs e)
		{
			var element = (UIElement) e.Element;

			element.MouseLeftButtonUp += ElementMouseLeftButtonUp;
			element.ManipulationStarted += ElementManipulationStarted;
			element.ManipulationDelta += ElementManipulationDelta;
		}

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

		public event EventHandler<NavigationListEventArgs> Navigation;

		protected void OnNavigation(NavigationListEventArgs args)
		{
			if (Navigation != null)
				Navigation(this, args);
		}
	}
}
