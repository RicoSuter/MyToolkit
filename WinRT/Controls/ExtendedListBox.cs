using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace MyToolkit.Controls
{
	public class ExtendedListBox : ListBox
	{
		//public ExtendedListBox()
		//{
		//	Background = new SolidColorBrush(Colors.Transparent);
		//}

		//public Thickness InnerMargin
		//{
		//	get { return Padding; }
		//	set { Padding = value; }
		//}

		public ListBoxItem GetListBoxItemFromItem(object item)
		{
			return (ListBoxItem)ItemContainerGenerator.ContainerFromItem(item);
		}

		#region prepare container for item event

		public event EventHandler<PrepareContainerForItemEventArgs> PrepareContainerForItem;
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
		{
			base.PrepareContainerForItemOverride(element, item);
			if (PrepareContainerForItem != null)
				PrepareContainerForItem(this, new PrepareContainerForItemEventArgs(element, item));
		}

		#endregion
	}

}
