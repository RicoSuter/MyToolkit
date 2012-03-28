using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyToolkit.Messages
{
	public class ActionMessage<T>
	{
		public Action<T> Action { get; set; }
	}
}
