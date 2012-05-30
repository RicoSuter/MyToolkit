using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using MyToolkit.Environment;
using MyToolkit.Utilities;

namespace MyToolkit.Paging
{
	public static class NavigationState
	{
		private static bool isNavigating = false; 
		public static bool IsNavigating
		{
			get
			{
				lock (typeof(NavigationState))
					return isNavigating;
			}
			set
			{
				lock (typeof(NavigationState))
					isNavigating = value;
			}
		}

		public static bool TryBeginNavigating()
		{
			lock (typeof(NavigationState))
			{
				if (IsNavigating)
					return false; 

				var page = PhoneApplication.CurrentPage as IExtendedPhoneApplicationPage;
				if (page != null)
				{
					SingleEvent.Register<IExtendedPhoneApplicationPage, EventArgs>(page,
						(p, e) => { p.NavigatedTo += e; },
						(p, e) => { p.NavigatedTo -= e; },
						delegate { IsNavigating = false; });
					IsNavigating = true;
				}

				return true; 
			}
		}
	}
}
