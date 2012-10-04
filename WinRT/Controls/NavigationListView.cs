using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace MyToolkit.Controls
{
	public class NavigationListView : ListView
	{
		//protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
		//{
		//	base.PrepareContainerForItemOverride(element, item);
		//	((UIElement)element).Tapped += OnTapped;
		//}

		//private void OnTapped(object sender, TappedRoutedEventArgs args)
		//{
		//	var element = (FrameworkElement)sender;
		//	OnNavigate(new NavigationListEventArgs(element.DataContext));
		//}

		//public event EventHandler<NavigationListEventArgs> Navigate;

		//protected void OnNavigate(NavigationListEventArgs args)
		//{
		//	var copy = Navigate;
		//	if (copy != null)
		//		copy(this, args);
		//}

		public NavigationListView()
		{
			SelectionChanged += OnSelectionChanged;
		}

		private void OnSelectionChanged(object sender, SelectionChangedEventArgs args)
		{
			var item = SelectedItem;
			SelectedItem = null;
			SelectedIndex = -1;
			if (item != null)
				OnNavigate(new NavigationListEventArgs(item));
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
