using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace MyToolkit.Controls
{
	public class NavigationList : ExtendedListBox
	{
		public NavigationList()
		{
			PrepareContainerForItem += OnPrepareContainerForItem;
		}

		private void OnPrepareContainerForItem(object sender, PrepareContainerForItemEventArgs e)
		{
			var element = (UIElement)e.Element;
			element.Tapped += OnTapped;
		}

		private void OnTapped(object sender, TappedRoutedEventArgs e)
		{
			//if (manipulationDeltaStarted)
			//	return;

			var element = (FrameworkElement)sender;
			OnNavigated(new NavigationListEventArgs(element.DataContext));
		}

		//private bool manipulationDeltaStarted;
		//private void ElementManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
		//{
		//	manipulationDeltaStarted = true;
		//}

		//private void ElementManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
		//{
		//	manipulationDeltaStarted = false;
		//}

		public event EventHandler<NavigationListEventArgs> Navigated;

		protected void OnNavigated(NavigationListEventArgs args)
		{
			var copy = Navigated;
			if (copy != null)
				copy(this, args);
		}
	}
}
