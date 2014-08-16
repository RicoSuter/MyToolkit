using System;
using Windows.UI.Xaml.Controls;

namespace MyToolkit.Controls
{
	public class NavigationListView : ExtendedListView
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
