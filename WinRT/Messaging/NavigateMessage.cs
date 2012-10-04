using System;
using MyToolkit.Paging;

namespace MyToolkit.Messaging
{
	public class NavigateMessage
	{
		public Type Page { get; private set; }
		public object Parameter { get; private set; }

		public NavigateMessage(Type page) : this(page, null) { } 
		public NavigateMessage(Type page, object parameter)
		{
			Page = page;
			Parameter = parameter; 
		}

		public static Action<NavigateMessage> GetAction(Frame frame)
		{
			return m => frame.Navigate(m.Page, m.Parameter);
		}
	}
}
