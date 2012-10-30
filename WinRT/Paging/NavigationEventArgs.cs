using System;
using Windows.UI.Xaml.Navigation;

namespace MyToolkit.Paging
{
	public class NavigationEventArgs
	{
		public object Content { get; internal set; }
		public object Parameter { get; internal set; }
		public Type SourcePageType { get; internal set; }
		public NavigationMode NavigationMode { get; internal set; }
	}

	public class NavigatingCancelEventArgs
	{
		public bool Cancel { get; set; }
		public object Content { get; internal set; }
		public NavigationMode NavigationMode { get; internal set; }
		public Type SourcePageType { get; internal set; }
	}
}