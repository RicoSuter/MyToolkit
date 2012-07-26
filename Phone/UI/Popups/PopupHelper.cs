using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using MyToolkit.Environment;
using MyToolkit.Paging;
using MyToolkit.Utilities;

namespace MyToolkit.UI.Popups
{
	public static class PopupHelper
	{
		public static void Show(IPopupControl control)
		{
			var oldState = new PageDeactivator();

			var page = PhonePage.CurrentPage; 
			var content = page.Content; 
			content.IsHitTestVisible = false;

			var popup = new System.Windows.Controls.Primitives.Popup { Child = (UIElement) control };
			popup.Width = ((FrameworkElement) content).ActualWidth;
			
			((FrameworkElement) control).Width = ((FrameworkElement) content).ActualWidth;

			var color = ColorUtility.RemoveAlpha(
				PhoneApplication.IsDarkTheme ? ColorUtility.FromHex("#22FFFFFF") : 
				ColorUtility.FromHex("#DDFFFFFF"), Colors.Black);

			var oldColor = SystemTray.BackgroundColor;
			
			control.SetBackgroundColor(color);
			oldState.DoIt(false);

			var del = new EventHandler<CancelEventArgs>(delegate(object sender, CancelEventArgs args)
			{
			    args.Cancel = true; 
				control.GoBack();
			});
			page.BackKeyPress += del;

			SystemTray.BackgroundColor = color;

			popup.IsOpen = true;
			control.Closed += delegate
			{
				content.IsHitTestVisible = true;
			    popup.IsOpen = false; 
				oldState.Revert();

				page.BackKeyPress -= del;

				SystemTray.BackgroundColor = oldColor;
			};
		}
	}
}