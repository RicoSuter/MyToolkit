using System;

namespace MyToolkit.Paging
{
	public enum NavigationMode
	{
		New = 0,
		Back = 1,
		Forward = 2,
		Refresh = 3,
	}

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