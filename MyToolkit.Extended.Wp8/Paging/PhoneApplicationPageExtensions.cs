using System;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using MyToolkit.Environment;

namespace MyToolkit.Paging
{
	public static class PhoneApplicationPageExtensions
	{
		public static bool TryNavigate(this PhoneApplicationPage page, Uri uri)
		{
			return TryNavigate(page, () => page.NavigationService.Navigate(uri));
		}

		public static bool TryNavigate(this PhoneApplicationPage page, Func<bool> navigationFunction)
		{
			lock (typeof(PhoneApplicationPageExtensions))
			{
				if (PhoneApplication.IsNavigating)
					return false;

				if (!(page is ExtendedPage)) // otherwise handled in ExtendedPage
				{
					var svc = page.NavigationService;
					svc.Navigated += OnNavigated;
				}

				if (navigationFunction())
				{
					PhoneApplication.IsNavigating = true;
					return true;
				}
				return false;
			}
		}
		
		private static void OnNavigated(object sender, NavigationEventArgs args)
		{
			((PhoneApplicationFrame)sender).Navigated -= OnNavigated;
			PhoneApplication.IsNavigating = false;
		}
	}
}
