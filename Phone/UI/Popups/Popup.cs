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

			var oldState = new PageDeactivator();
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