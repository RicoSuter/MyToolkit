using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyToolkit.Utilities;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace MyToolkit.UI
{
	public static class PopupHelper
	{
		private static int openPopups = 0;
		public static bool IsPopupVisible { get { return openPopups > 0; } }

		public static Popup GetParentPopup(FrameworkElement element)
		{
			return element.GetVisualAncestors().LastOrDefault() as Popup; 
		}

		public static bool IsInPopup(FrameworkElement element)
		{
			if (element is Popup)
				return true;
			return GetParentPopup(element) != null; 
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
			var bounds = Window.Current.CoreWindow.Bounds;
			var parent = (FrameworkElement)Window.Current.Content;

			if (isHorizontal)
				control.Width = bounds.Width;
			else
				control.Height = bounds.Height;

			var popup = new Popup();
			var del = new WindowActivatedEventHandler((sender, e) =>
			{
				control.Width = bounds.Width;
				if (isHorizontal)
					popup.VerticalOffset = bounds.Top + (bounds.Height - control.ActualHeight) / 2;
				else
					popup.HorizontalOffset = bounds.Left + (bounds.Width - control.ActualWidth) / 2;
			});

			var del2 = new SizeChangedEventHandler((sender, e) =>
			{
				if (isHorizontal)
					popup.VerticalOffset = bounds.Top + (bounds.Height - control.ActualHeight) / 2;
				else
					popup.HorizontalOffset = bounds.Left + (bounds.Width - control.ActualWidth) / 2;
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
				openPopups--;
			};
			openPopups++;
			popup.IsOpen = true;

			popup.Tag = 0.0;
			InputPane.GetForCurrentView().Showing += (s, args) => UpdateElementLocation(popup);
			InputPane.GetForCurrentView().Hiding += (s, args) =>
			{
				popup.VerticalOffset += (double)popup.Tag;
				popup.Tag = 0.0;
			};
			return popup;
		}

		private static void UpdateElementLocation(Popup popup)
		{
			var occlutedRect = InputPane.GetForCurrentView().OccludedRect;
			if (occlutedRect.Top > 0)
			{
				var element = FocusManager.GetFocusedElement() as FrameworkElement;
				if (element != null)
				{
					SingleEvent.Register(element,
						(e, h) => e.LostFocus += h,
						(e, h) => e.LostFocus -= h,
						delegate { UpdateElementLocation(popup); });

					var point = element.TransformToVisual(Window.Current.Content).TransformPoint(new Point(0, 0));
					if (point.X + element.ActualHeight + 100 > occlutedRect.Top)
					{
						var offset = (point.X + element.ActualHeight + 100) - occlutedRect.Top - (double)popup.Tag;
						if (offset > 20)
						{
							popup.VerticalOffset -= offset;
							popup.Tag = (double)popup.Tag + offset;
						}
					}
				}
			}
		}

		public static Popup ShowSettings(FrameworkElement control, Action<Popup> closed = null)
		{
			var bounds = Window.Current.CoreWindow.Bounds;
			control.Height = bounds.Height;

			var popup = new Popup();
			var del1 = new WindowActivatedEventHandler((sender, e) =>
			{
				if (e.WindowActivationState == CoreWindowActivationState.Deactivated)
					popup.IsOpen = false;
			});
			var del2 = new SizeChangedEventHandler((sender, e) =>
			{
				popup.HorizontalOffset = bounds.Left + (bounds.Width - control.ActualWidth);
			});

			Window.Current.Activated += del1;
			control.SizeChanged += del2;

			popup.IsLightDismissEnabled = true;
			popup.Child = control;
			popup.Closed += delegate 
			{
				Window.Current.Activated -= del1;
				control.SizeChanged -= del2;
				if (closed != null)
					closed(popup);
				openPopups--;
			};
			openPopups++;
			popup.IsOpen = true;
			return popup;
		}

		public static Popup ShowPane(FrameworkElement control, bool left = true, Action<Popup> closed = null)
		{
			var bounds = Window.Current.CoreWindow.Bounds;
			control.Height = bounds.Height;

			var popup = new Popup();
			var del = new WindowActivatedEventHandler((sender, e) =>
			{
				if (e.WindowActivationState == CoreWindowActivationState.Deactivated)
					popup.IsOpen = false;
			});
			var del2 = new SizeChangedEventHandler((sender, e) =>
			{
				if (!left)
					popup.HorizontalOffset = bounds.Left + (bounds.Width - control.ActualWidth);
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
				openPopups--;
			};
			openPopups++;
			popup.IsOpen = true;
			return popup;
		}
	}
}
