using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Phone.Shell;
using MyToolkit.Phone;
using MyToolkit.Utilities;

namespace MyToolkit.UI.Popups
{
	public static class Popup
	{
		public static void Show(IPopupControl control)
		{
			// TODO animations
			// TODO check if ApplicationBar is available (!= null)

			var oldTrayBg = SystemTray.BackgroundColor;
			var oldTrayFg = SystemTray.ForegroundColor;
			var oldControl = PhoneApplication.CurrentPage.Content;
			var oldControlOpacity = oldControl.Opacity;

			var oldBarBgColor = PhoneApplication.CurrentPage.ApplicationBar.BackgroundColor;
			var oldBarMenu = PhoneApplication.CurrentPage.ApplicationBar.IsMenuEnabled;

			PhoneApplication.CurrentPage.Content = null;

			var grid = new Grid();
			grid.Children.Add(oldControl);
			grid.Children.Add((UIElement)control);

			oldControl.Opacity = 0.325;

			SystemTray.BackgroundColor = ColorUtility.RemoveAlpha(
				PhoneApplication.IsDarkTheme ? ColorUtility.FromHex("#22FFFFFF") : ColorUtility.FromHex("#DDFFFFFF"), Colors.Black);

			SystemTray.ForegroundColor = Resources.PhoneForegroundColor;
			control.SetBackgroundColor(SystemTray.BackgroundColor);

			PhoneApplication.CurrentPage.ApplicationBar.BackgroundColor = ColorUtility.Mix(oldBarBgColor, 0.325, Resources.PhoneBackgroundColor);
			PhoneApplication.CurrentPage.ApplicationBar.IsMenuEnabled = false;

			var oldEnabledButtons = new List<ApplicationBarIconButton>();
			foreach (var b in PhoneApplication.CurrentPage.ApplicationBar.Buttons.
				OfType<ApplicationBarIconButton>().Where(i => i.IsEnabled))
			{
				b.IsEnabled = false;
				oldEnabledButtons.Add(b);
			}

			var oldEnabledMenus = new List<ApplicationBarMenuItem>();
			foreach (var b in PhoneApplication.CurrentPage.ApplicationBar.MenuItems.
				OfType<ApplicationBarMenuItem>().Where(i => i.IsEnabled))
			{
				b.IsEnabled = false;
				oldEnabledMenus.Add(b);
			}

			// TODO unregister other events and add them again on close
			PhoneApplication.CurrentPage.BackKeyPress += (s, e) => 
			{ 
				e.Cancel = true;
				Deployment.Current.Dispatcher.BeginInvoke(control.GoBack);
			};

			PhoneApplication.CurrentPage.Content = grid;

			control.Closed += delegate
			{
				oldControl.Opacity = oldControlOpacity;
				SystemTray.BackgroundColor = oldTrayBg;
				SystemTray.ForegroundColor = oldTrayFg;

				PhoneApplication.CurrentPage.ApplicationBar.BackgroundColor = oldBarBgColor;
				PhoneApplication.CurrentPage.ApplicationBar.IsMenuEnabled = oldBarMenu;
				foreach (var b in oldEnabledButtons)
					b.IsEnabled = true;
				foreach (var b in oldEnabledMenus)
					b.IsEnabled = true;

				grid.Children.Remove((UIElement)control);
				grid.Children.Remove(oldControl);
				PhoneApplication.CurrentPage.Content = oldControl;
			};
		}
	}
}