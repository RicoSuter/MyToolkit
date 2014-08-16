using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace MyToolkit.Controls
{
	public class NavigationGridView : GridView
	{
		public NavigationGridView()
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
