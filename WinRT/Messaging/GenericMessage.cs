using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyToolkit.Messaging
{
	public class GenericMessage<T>
	{
		public object Parameter { get; private set; }

		public GenericMessage(object parameter)
		{
			Parameter = parameter; 
		} 
	}
}
