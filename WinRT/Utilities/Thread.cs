using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyToolkit.Utilities
{
	public class Thread
	{
		public static void Sleep(int ms)
		{
			new System.Threading.ManualResetEvent(false).WaitOne(ms);
		}
	}
}
