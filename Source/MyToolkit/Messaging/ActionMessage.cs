using System;

namespace MyToolkit.Messaging
{
	public class ActionMessage<T>
	{
		public Action<T> Action { get; set; }
	}
}
