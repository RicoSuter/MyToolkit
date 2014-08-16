using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace MyToolkit.Controls
{
	public class NavigationList : ScrollableItemsControl
	{
        //public NavigationList()
        //{
        //    Foreground = new SolidColorBrush(Colors.White);
        //}

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

        /// <summary>
        /// Occurs when the user clicked on an item and wants to navigate to its detail page. 
        /// </summary>
		public event EventHandler<NavigationListEventArgs> Navigate;

		protected void OnNavigate(NavigationListEventArgs args)
		{
			var copy = Navigate;
			if (copy != null)
				copy(this, args);
		}
	}
}
