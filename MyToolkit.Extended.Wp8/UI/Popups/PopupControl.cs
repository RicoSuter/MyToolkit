using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Microsoft.Phone.Shell;
using MyToolkit.Environment;
using MyToolkit.Paging;
using MyToolkit.Utilities;

namespace MyToolkit.UI.Popups
{
	public abstract class PopupControl : UserControl
	{
		#region Static

		public void Show()
		{
			Show(this);
		}

		public static Popup Show(PopupControl control)
		{
			return Show(control, false, false, null);
		}

		public Task ShowAsync()
		{
			return ShowAsync(this, false, false);
		}

		public Task ShowAsync(bool hideApplicationBar, bool showFullScreen)
		{
			return ShowAsync(this, hideApplicationBar, showFullScreen);
		}

		public static Task ShowAsync(PopupControl control, bool hideApplicationBar, bool showFullScreen)
		{
			return TaskHelper.RunCallbackMethod<PopupControl, bool, bool, PopupControl>(
				(a, b, c, d) => Show(a, b, c, d), 
				control, hideApplicationBar, showFullScreen);
		}
		
		private static Stack<PopupControl> popupStack; 
		public static Popup Show(PopupControl control, bool hideApplicationBar, bool showFullScreen, Action<PopupControl> completed)
		{
			if (popupStack == null)
				popupStack = new Stack<PopupControl>();

			var oldState = new PageDeactivator();
			var page = PhoneApplication.CurrentPage;

			var heightSub = 0.0;
			if (hideApplicationBar && page.ApplicationBar != null && page.ApplicationBar.IsVisible)
			{
				page.ApplicationBar.IsVisible = false;
				heightSub = page.ApplicationBar.Mode == ApplicationBarMode.Minimized
					? page.ApplicationBar.MiniSize
					: page.ApplicationBar.DefaultSize;
			}
			else
				hideApplicationBar = false;

			var content = page.Content;
			content.IsHitTestVisible = false;

			var popup = new Popup
			{
				Child = control,
				HorizontalAlignment = HorizontalAlignment.Stretch,
				VerticalAlignment = VerticalAlignment.Stretch
			};

			control.Popup = popup;

			if (SystemTray.IsVisible && !(SystemTray.Opacity < 1.0))
				heightSub += 32; 

			// set popup size
			if (showFullScreen)
				popup.Height = page.ActualHeight + heightSub;
			popup.Width = page.ActualWidth;

			// set control size
			if (showFullScreen)
				control.Height = page.ActualHeight + heightSub;
			control.Width = page.ActualWidth;

			// hide underlying page
			if (showFullScreen && content.Visibility == Visibility.Visible)
				content.Visibility = Visibility.Collapsed;
			else
				showFullScreen = false; 

			// deactivate page
			var color = ColorUtility.RemoveAlpha(
				PhoneApplication.IsDarkTheme ? ColorUtility.FromHex("#22FFFFFF") :
				ColorUtility.FromHex("#DDFFFFFF"), Colors.Black);
			var oldColor = SystemTray.BackgroundColor;

			control.SetBackgroundColor(color);
			oldState.DoIt(false);

			// register back key event
			object handlerObject = null; 
			if (page is ExtendedPage)
			{
				handlerObject = ((ExtendedPage)page).AddBackKeyPressHandler(args =>
				{
					if (popupStack.Peek() == control)
					{
						args.Cancel = true;
						control.GoBack();
					}
				});
			}
			else
			{
				var handler = new EventHandler<CancelEventArgs>((sender, args) =>
				{
					if (popupStack.Peek() == control)
					{
						args.Cancel = true;
						control.GoBack();
					}
				});
				page.BackKeyPress += handler;
				handlerObject = handler;
			}

			SystemTray.BackgroundColor = color;

			popup.IsOpen = true;

			// hide underlying popups
			foreach (var p in popupStack)
				p.Visibility = Visibility.Collapsed;
			popupStack.Push(control);

			control.Closed += delegate
			{
				if (showFullScreen)
					content.Visibility = Visibility.Visible;

				if (hideApplicationBar && page.ApplicationBar != null)
					page.ApplicationBar.IsVisible = true;

				content.IsHitTestVisible = true;

				popup.IsOpen = false;

				popupStack.Pop();
				if (popupStack.Count > 0)
					popupStack.Peek().Visibility = Visibility.Visible; 

				oldState.Revert();

				if (page is ExtendedPage)
					((ExtendedPage)page).RemoveBackKeyPressAsyncHandler((Func<CancelEventArgs, Task>) handlerObject);
				else
					page.BackKeyPress -= (EventHandler<CancelEventArgs>)handlerObject;
				
				SystemTray.BackgroundColor = oldColor;
				if (completed != null)
					completed(control);
			};

			return popup;
		}

		#endregion

		public abstract void GoBack();

		public event Action<PopupControl> Closed;
		public Popup Popup { get; internal set; }
		
		public virtual void SetBackgroundColor(Color color)
		{
			if (Content is Control)
				((Control)Content).Background = new SolidColorBrush(color);
			else if (Content is Panel)
				((Panel)Content).Background = new SolidColorBrush(color);
			else
				throw new NotImplementedException();
		}

		public void Close()
		{
			var copy = Closed;
			if (copy != null)
				Closed(this);
		}
	}
}