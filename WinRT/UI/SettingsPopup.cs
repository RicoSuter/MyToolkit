using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;

namespace MyToolkit.UI
{
	public static class SettingsPopup
	{
		public static void Show(FrameworkElement popupControl, Action closed = null)
		{
			Show(popupControl, popupControl.Width, closed);
		}

		public static void Show(FrameworkElement popupControl, double width, Action closed = null)
		{
			var parent = (FrameworkElement)Window.Current.Content;
			popupControl.Height = parent.ActualHeight;

			var popup = new Popup();
			var del = new WindowActivatedEventHandler((sender, e) =>
			{
				if (e.WindowActivationState == CoreWindowActivationState.Deactivated)
					popup.IsOpen = false;
			});

			Window.Current.Activated += del;

			popup.IsLightDismissEnabled = true;
			popup.Child = popupControl;
			popup.HorizontalOffset = parent.ActualWidth - width;
			popup.Closed += delegate 
			{ 
				Window.Current.Activated -= del;
				if (closed != null)
					closed();
			};
			popup.IsOpen = true;
		}
	}
}
