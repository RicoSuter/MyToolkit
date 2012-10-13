using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyToolkit.Controls
{
	public class ExtendedListBox : ListBox
	{
		public ExtendedListBox()
		{
			DefaultStyleKey = typeof(ExtendedListBox);
		}

		//public Thickness InnerMargin
		//{
		//	get { return Padding; }
		//	set { Padding = value; }
		//}

		public ListBoxItem GetListBoxItemFromItem(object item)
		{
			return (ListBoxItem)ItemContainerGenerator.ContainerFromItem(item);
		}

		public event EventHandler<PrepareContainerForItemEventArgs> PrepareContainerForItem;
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
		{
			base.PrepareContainerForItemOverride(element, item);
			if (PrepareContainerForItem != null)
				PrepareContainerForItem(this, new PrepareContainerForItemEventArgs(element, item));
		}
	}
}
