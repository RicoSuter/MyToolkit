using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyToolkit.Utilities
{
	public static class TaskHelperEx
	{
		public static Task RunOnDispatcherAsync(Action completed)
		{
			var task = new TaskCompletionSource<object>();
			Deployment.Current.Dispatcher.BeginInvoke(delegate
			{
				try
				{
					completed();
				}
				finally
				{
					task.SetResult(null);
				}
			});
			return task.Task;
		} 
	}
}
