using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using MyToolkit.Phone;
using MyToolkit.Utilities;

namespace MyToolkit.UI.Popups
{
	public class OldPopupState
	{
		public UIElement OldControl { get; private set; }
		private double oldControlOpacity;
		private Color oldTrayBg;
		private Color oldTrayFg;
		private Color oldBarBgColor;
		private bool oldBarMenu;
		private List<ApplicationBarMenuItem> oldEnabledMenus;
		private List<ApplicationBarIconButton> oldEnabledButtons;
		private PhoneApplicationPage page;
		private bool makePageInactive;

		public static OldPopupState Inactivate()
		{
			return Inactivate(true);
		}

		public static OldPopupState Inactivate(bool makePageInactive)
		{
			var s = new OldPopupState();
			s.DoIt(makePageInactive);
			return s; 
		}

		public void DoIt(bool makePageInactive)
		{
			this.makePageInactive = makePageInactive;
			page = PhoneApplication.CurrentPage;

			OldControl = page.Content;
			oldControlOpacity = OldControl.Opacity;

			oldTrayBg = SystemTray.BackgroundColor;
			oldTrayFg = SystemTray.ForegroundColor;

			oldBarBgColor = page.ApplicationBar.BackgroundColor;
			oldBarMenu = page.ApplicationBar.IsMenuEnabled;

			OldControl.Opacity = 0.325;

			SystemTray.BackgroundColor = Resources.PhoneBackgroundColor;
			SystemTray.ForegroundColor = Resources.PhoneForegroundColor;

			page.ApplicationBar.BackgroundColor = ColorUtility.Mix(oldBarBgColor, 0.325, Resources.PhoneBackgroundColor);
			page.ApplicationBar.IsMenuEnabled = false;

			oldEnabledButtons = new List<ApplicationBarIconButton>();
			foreach (var b in page.ApplicationBar.Buttons.
				OfType<ApplicationBarIconButton>().Where(i => i.IsEnabled))
			{
				b.IsEnabled = false;
				oldEnabledButtons.Add(b);
			}

			oldEnabledMenus = new List<ApplicationBarMenuItem>();
			foreach (var b in page.ApplicationBar.MenuItems.
				OfType<ApplicationBarMenuItem>().Where(i => i.IsEnabled))
			{
				b.IsEnabled = false;
				oldEnabledMenus.Add(b);
			}

			if (makePageInactive)
				page.IsEnabled = false; 
		}

		public void Revert()
		{
			OldControl.Opacity = oldControlOpacity;
			SystemTray.BackgroundColor = oldTrayBg;
			SystemTray.ForegroundColor = oldTrayFg;

			page.ApplicationBar.BackgroundColor = oldBarBgColor;
			page.ApplicationBar.IsMenuEnabled = oldBarMenu;

			foreach (var b in oldEnabledButtons)
				b.IsEnabled = true;
			foreach (var b in oldEnabledMenus)
				b.IsEnabled = true;

			if (makePageInactive)
				page.IsEnabled = true; 
		}
	}

	public static class Popup
	{
		public static void Show(IPopupControl control)
		{
			// TODO animations
			// TODO check if ApplicationBar is available (!= null)

			var oldState = new OldPopupState();
			PhoneApplication.CurrentPage.Content = null;

			var grid = new Grid();
			grid.Children.Add(oldState.OldControl);
			grid.Children.Add((UIElement)control);

			control.SetBackgroundColor(SystemTray.BackgroundColor);

			oldState.DoIt(false);
			SystemTray.BackgroundColor = ColorUtility.RemoveAlpha(
				PhoneApplication.IsDarkTheme ? ColorUtility.FromHex("#22FFFFFF") : ColorUtility.FromHex("#DDFFFFFF"), Colors.Black);
			PhoneApplication.CurrentPage.Content = grid;

			control.Closed += delegate
			{
				oldState.Revert();
				grid.Children.Remove((UIElement)control);
				grid.Children.Remove(oldState.OldControl);
				PhoneApplication.CurrentPage.Content = oldState.OldControl;
			};
		}
	}
}