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
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
		{
			base.PrepareContainerForItemOverride(element, item);
			((UIElement)element).Tapped += OnTapped;
		}

		private void OnTapped(object sender, TappedRoutedEventArgs e)
		{
			var element = (FrameworkElement)sender;
			OnNavigated(new NavigationListEventArgs(element.DataContext));
		}

		public event EventHandler<NavigationListEventArgs> Navigated;

		protected void OnNavigated(NavigationListEventArgs args)
		{
			var copy = Navigated;
			if (copy != null)
				copy(this, args);
		}
	}
}
