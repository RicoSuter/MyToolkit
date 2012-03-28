using System;

namespace MyToolkit.Controls
{
	public class NavigationListEventArgs : EventArgs
	{
		internal NavigationListEventArgs(object item)
		{
			Item = item;
		}

		public object Item { private set; get; }
	}
}