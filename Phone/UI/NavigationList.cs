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
	public class NavigationList : ExtendedListBox
	{
		static NavigationList()
		{
			if (!TiltEffect.TiltableItems.Contains(typeof(ContentPresenter)))
				TiltEffect.TiltableItems.Add(typeof(ContentPresenter));
		}

		public NavigationList()
		{
			PrepareContainerForItem += OnPrepareContainerForItem;
		}

		private void OnPrepareContainerForItem(object sender, PrepareContainerForItemEventArgs e)
		{
			var element = (UIElement) e.Element;

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

		public event EventHandler<NavigationListEventArgs> Navigation;

		protected void OnNavigation(NavigationListEventArgs args)
		{
			if (Navigation != null)
				Navigation(this, args);
		}
	}
}
