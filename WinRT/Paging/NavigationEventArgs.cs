using System;
using Windows.UI.Xaml.Navigation;

namespace MyToolkit.Paging
{
	public class NavigationEventArgs
	{
		public object Content { get; set; }
		public object Parameter { get; set; }
		public Type Type { get; set; }
		public NavigationMode NavigationMode { get; set; }
	}
}