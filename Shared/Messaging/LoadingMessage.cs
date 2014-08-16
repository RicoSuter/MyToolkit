using System;

namespace MyToolkit.Messaging
{
	public class LoadingMessage
	{
		public LoadingMessage(bool isLoading)
		{
			IsLoading = isLoading; 
		}

		public bool IsLoading { get; set; }
	}
}
