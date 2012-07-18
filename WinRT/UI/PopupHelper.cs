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
		public static Task<Popup> ShowDialogAsync<T>(T control, bool isLightDismissEnabled = false) where T : FrameworkElement
		{
			return TaskHelper.RunCallbackMethod<T, Popup>((x, y) => ShowDialog(x, isLightDismissEnabled, y), control);
		}

		public static Popup ShowDialog<T>(T control, bool isLightDismissEnabled = false, Action<Popup> closed = null) where T : FrameworkElement
		{
			var parent = (FrameworkElement)Window.Current.Content;
			control.Width = parent.ActualWidth;

			var popup = new Popup();
			var del = new WindowActivatedEventHandler((sender, e) =>
			{
				control.Width = parent.ActualWidth;
				popup.VerticalOffset = (parent.ActualHeight - control.ActualHeight) / 2;
			});

			var del2 = new SizeChangedEventHandler((sender, e) =>
			{
				popup.VerticalOffset = (parent.ActualHeight - control.ActualHeight) / 2;
			});

			Window.Current.Activated += del;
			control.SizeChanged += del2;

			var oldOpacity = parent.Opacity;
			parent.Opacity = 0.5; 
			parent.IsHitTestVisible = false;

			popup.Child = control;
			popup.IsLightDismissEnabled = isLightDismissEnabled;
			popup.Closed += delegate
			{
				parent.Opacity = oldOpacity; 
				parent.IsHitTestVisible = true;

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
