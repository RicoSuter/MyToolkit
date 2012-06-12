using System;

namespace MyToolkit.Messaging
{
	public class GoBackMessage
	{
		public GoBackMessage() { }
		public GoBackMessage(Action<bool> completed)
		{
			Completed = completed;
		}

		/// <summary>
		/// the boolean in the action is the successful parameter
		/// </summary>
		public Action<bool> Completed { get; set; }
	}
}
