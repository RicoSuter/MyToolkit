using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using MyToolkit.Environment;
using MyToolkit.Paging;
using MyToolkit.Utilities;

namespace MyToolkit.UI
{
	public class PageDeactivator
	{
		public UIElement OldControl
		{
			get;
			private set;
		}
		private double oldControlOpacity;
		private Color oldTrayBg;
		private Color oldTrayFg;
		private Color oldBarBgColor;
		private bool oldBarMenu;
		private List<ApplicationBarMenuItem> oldEnabledMenus;
		private List<ApplicationBarIconButton> oldEnabledButtons;
		private PhoneApplicationPage page;
		private bool disableInteractions;

		public static PageDeactivator Inactivate()
		{
			return Inactivate(true);
		}

		public static PageDeactivator Inactivate(bool makePageInactive)
		{
			var s = new PageDeactivator();
			s.DoIt(makePageInactive);
			return s;
		}

		internal void DoIt(bool disablePageInteractions)
		{
			disableInteractions = disablePageInteractions;
			page = PhoneApplication.CurrentPage;

            OldControl = page.Content;
            oldControlOpacity = OldControl.Opacity;

			oldTrayBg = SystemTray.BackgroundColor;
			oldTrayFg = SystemTray.ForegroundColor;

			if(page.ApplicationBar != null)
			{
				oldBarBgColor = page.ApplicationBar.BackgroundColor;
				oldBarMenu = page.ApplicationBar.IsMenuEnabled;
			}

			OldControl.Opacity = 0.325;

			SystemTray.BackgroundColor = Environment.Resources.PhoneBackgroundColor;
			SystemTray.ForegroundColor = Environment.Resources.PhoneForegroundColor;

			oldEnabledButtons = new List<ApplicationBarIconButton>();
			oldEnabledMenus = new List<ApplicationBarMenuItem>();
			if(page.ApplicationBar != null)
			{
				page.ApplicationBar.BackgroundColor = ColorUtility.Mix(oldBarBgColor, 0.325, Environment.Resources.PhoneBackgroundColor);
				page.ApplicationBar.IsMenuEnabled = false;
				foreach(var b in page.ApplicationBar.Buttons.
					OfType<ApplicationBarIconButton>().Where(i => i.IsEnabled))
				{
					b.IsEnabled = false;
					oldEnabledButtons.Add(b);
				}

				foreach(var b in page.ApplicationBar.MenuItems.
					OfType<ApplicationBarMenuItem>().Where(i => i.IsEnabled))
				{
					b.IsEnabled = false;
					oldEnabledMenus.Add(b);
				}
			}

			if(disableInteractions)
				page.IsEnabled = false;
		}

		public void Revert()
		{
			OldControl.Opacity = oldControlOpacity;
			SystemTray.BackgroundColor = oldTrayBg;
			SystemTray.ForegroundColor = oldTrayFg;

			if(page.ApplicationBar != null)
			{
				page.ApplicationBar.BackgroundColor = oldBarBgColor;
				page.ApplicationBar.IsMenuEnabled = oldBarMenu;

				foreach(var b in oldEnabledButtons)
					b.IsEnabled = true;
				foreach(var b in oldEnabledMenus)
					b.IsEnabled = true;
			}

			if(disableInteractions)
				page.IsEnabled = true;
		}
	}
}