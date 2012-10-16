using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyToolkit.Utilities;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace MyToolkit.UI
{
	public static class PopupHelper
	{
		public static bool IsInPopup(FrameworkElement element)
		{
			if (element is Popup)
				return true;

			var last = element.GetVisualAncestors().LastOrDefault();
			if (last != null && last.Parent is Popup)
				return true;

			return false; 
		}

		public static void ClosePopup(this FrameworkElement control)
		{
			((Popup) control.Tag).IsOpen = false; 
		}

		public static Task<Popup> ShowDialogAsync(FrameworkElement control, bool isLightDismissEnabled = false, bool isHorizontal = true)
		{
			return TaskHelper.RunCallbackMethod<FrameworkElement, Popup>((x, y) => ShowDialog(x, isLightDismissEnabled, isHorizontal, y), control);
		}

		public static Popup ShowDialog(FrameworkElement control, bool isLightDismissEnabled = false, bool isHorizontal = true, Action<Popup> closed = null)
		{
			var parent = (FrameworkElement)Window.Current.Content;

			if (isHorizontal)
				control.Width = parent.ActualWidth;
			else
				control.Height = parent.ActualHeight;

			var popup = new Popup();
			var del = new WindowActivatedEventHandler((sender, e) =>
			{
				control.Width = parent.ActualWidth;
				if (isHorizontal)
					popup.VerticalOffset = (parent.ActualHeight - control.ActualHeight) / 2;
				else
					popup.HorizontalOffset = (parent.ActualWidth - control.ActualWidth) / 2;
			});

			var del2 = new SizeChangedEventHandler((sender, e) =>
			{
				if (isHorizontal)
					popup.VerticalOffset = (parent.ActualHeight - control.ActualHeight) / 2;
				else
					popup.HorizontalOffset = (parent.ActualWidth - control.ActualWidth) / 2;
			});

			Window.Current.Activated += del;
			control.SizeChanged += del2;
			control.Tag = popup; 

			var oldOpacity = parent.Opacity;
			parent.Opacity = 0.5; 
			parent.IsHitTestVisible = false;

			var topAppBarVisibility = Visibility.Collapsed;
			var bottomAppBarVisibility = Visibility.Collapsed;
			if (parent is Paging.Frame)
			{
				var page = ((Paging.Frame)parent).Content as Paging.Page;
				if (page != null)
				{
					if (page.TopAppBar != null)
					{
						topAppBarVisibility = page.TopAppBar.Visibility;
						page.TopAppBar.Visibility = Visibility.Collapsed;
					}
					if (page.BottomAppBar != null)
					{
						bottomAppBarVisibility = page.BottomAppBar.Visibility;
						page.BottomAppBar.Visibility = Visibility.Collapsed;
					}
				}
			}

			popup.Child = control;
			popup.IsLightDismissEnabled = isLightDismissEnabled;
			popup.Closed += delegate
			{
				parent.Opacity = oldOpacity; 
				parent.IsHitTestVisible = true;

				if (parent is Paging.Frame)
				{
					var page = ((Paging.Frame)parent).Content as Paging.Page;
					if (page != null)
					{
						if (page.TopAppBar != null)
							page.TopAppBar.Visibility = topAppBarVisibility;
						if (page.BottomAppBar != null)
							page.BottomAppBar.Visibility = bottomAppBarVisibility;
					}
				}

				Window.Current.Activated -= del;
				control.SizeChanged -= del2;

				if (closed != null)
					closed(popup);
			};
			popup.IsOpen = true;
			return popup;
		}

		public static Popup ShowSettings(FrameworkElement control, Action<Popup> closed = null)
		{
			var parent = (FrameworkElement)Window.Current.Content;
			control.Height = parent.ActualHeight;

			var popup = new Popup();
			var del = new WindowActivatedEventHandler((sender, e) =>
			{
				if (e.WindowActivationState == CoreWindowActivationState.Deactivated)
					popup.IsOpen = false;
			});
			var del2 = new SizeChangedEventHandler((sender, e) =>
			{
				popup.HorizontalOffset = parent.ActualWidth - control.ActualWidth;
			});

			Window.Current.Activated += del;
			control.SizeChanged += del2;

			popup.IsLightDismissEnabled = true;
			popup.Child = control;
			popup.Closed += delegate 
			{
				Window.Current.Activated -= del;
				control.SizeChanged -= del2;
				if (closed != null)
					closed(popup);
			};
			popup.IsOpen = true; 
			return popup;
		}

		public static Popup ShowPane(FrameworkElement control, bool left = true, Action<Popup> closed = null)
		{
			var parent = (FrameworkElement)Window.Current.Content;
			control.Height = parent.ActualHeight;

			var popup = new Popup();
			var del = new WindowActivatedEventHandler((sender, e) =>
			{
				if (e.WindowActivationState == CoreWindowActivationState.Deactivated)
					popup.IsOpen = false;
			});
			var del2 = new SizeChangedEventHandler((sender, e) =>
			{
				if (!left)
					popup.HorizontalOffset = parent.ActualWidth - control.ActualWidth;
			});

			Window.Current.Activated += del;
			control.SizeChanged += del2;

			popup.IsLightDismissEnabled = true;
			popup.Child = control;
			popup.Closed += delegate
			{
				Window.Current.Activated -= del;
				control.SizeChanged -= del2;
				if (closed != null)
					closed(popup);
			};
			popup.IsOpen = true;
			return popup;
		}
	}
}
