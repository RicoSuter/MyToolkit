using System;
using System.Windows;

namespace MyToolkit.Controls
{
	public class PrepareContainerForItemEventArgs : EventArgs
	{
		public PrepareContainerForItemEventArgs(DependencyObject element, object item)
		{
			Element = element;
			Item = item;
		}

		public DependencyObject Element { get; private set; }
		public object Item { get; private set; }
	}
}