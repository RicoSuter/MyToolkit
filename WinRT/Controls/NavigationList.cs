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
			OnNavigate(new NavigationListEventArgs(element.DataContext));
		}

		public event EventHandler<NavigationListEventArgs> Navigate;

		protected void OnNavigate(NavigationListEventArgs args)
		{
			var copy = Navigate;
			if (copy != null)
				copy(this, args);
		}
	}
}
