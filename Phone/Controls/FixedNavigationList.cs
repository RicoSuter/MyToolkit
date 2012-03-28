using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Phone.Controls;

namespace MyToolkit.Controls
{
	public class FixedNavigationList : ExtendedItemsControl
	{
		public event EventHandler<NavigationListEventArgs> Navigation;

		static FixedNavigationList()
		{
			if (!TiltEffect.TiltableItems.Contains(typeof(ContentPresenter)))
				TiltEffect.TiltableItems.Add(typeof(ContentPresenter));
		}

		public FixedNavigationList()
		{
			PrepareContainerForItem += OnPrepareContainerForItem; 
		}

		private void OnPrepareContainerForItem(object sender, PrepareContainerForItemEventArgs e)
		{
			var element = (UIElement)e.Element;
			element.MouseLeftButtonUp += ElementMouseLeftButtonUp;
			element.ManipulationStarted += ElementManipulationStarted;
			element.ManipulationDelta += ElementManipulationDelta;
		}

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
	}
}