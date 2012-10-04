using System;
using MyToolkit.Paging;

namespace MyToolkit.Messaging
{
	public class GoBackMessage
	{
		public static Action<GoBackMessage> GetAction(Frame frame)
		{
			return m => frame.GoBack();
		}
	}
}
