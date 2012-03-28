using System;
using System.Windows;
using System.Windows.Controls;

namespace MyToolkit.Controls
{
	public class ExtendedItemsControl : ItemsControl
	{
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
		{
			base.PrepareContainerForItemOverride(element, item);
			OnPrepareContainerForItem(new PrepareContainerForItemEventArgs(element, item));
		}

		public event EventHandler<PrepareContainerForItemEventArgs> PrepareContainerForItem;
		protected void OnPrepareContainerForItem(PrepareContainerForItemEventArgs args)
		{
			if (PrepareContainerForItem != null)
				PrepareContainerForItem(this, args);
		}
	}
}