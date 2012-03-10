using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyToolkit.Utilities
{
	public static class TaskHelper
	{
		public static Task<TResult> RunCallbackMethod<TResult>(Action<Action<TResult>> func)
		{
			var task = new TaskCompletionSource<TResult>();
			func(result => { task.SetResult(result); });
			return task.Task;
		}

		public static Task<TResult> RunCallbackMethod<T1, TResult>(Action<T1, Action<TResult>> func, T1 a)
		{
			var task = new TaskCompletionSource<TResult>();
			func(a, result => { task.SetResult(result); });
			return task.Task;
		}

		public static Task<TResult> RunCallbackMethod<T1, T2, TResult>(Action<T1, T2, Action<TResult>> func, T1 a, T2 b)
		{
			var task = new TaskCompletionSource<TResult>();
			func(a, b, result => { task.SetResult(result); });
			return task.Task;
		}

		public static Task<TResult> RunCallbackMethod<T1, T2, T3, TResult>(Action<T1, T2, T3, Action<TResult>> func, T1 a, T2 b, T3 c)
		{
			var task = new TaskCompletionSource<TResult>();
			func(a, b, c, result => { task.SetResult(result); });
			return task.Task;
		}

		public static Task<TResult> RunCallbackMethod<T1, T2, T3, T4, TResult>(Action<T1, T2, T3, T4, Action<TResult>> func, T1 a, T2 b, T3 c, T4 d)
		{
			var task = new TaskCompletionSource<TResult>();
			func(a, b, c, d, result => { task.SetResult(result); });
			return task.Task;
		}

		public static Task<TResult> RunCallbackMethod<T1, T2, T3, T4, T5, TResult>(Action<T1, T2, T3, T4, T5, Action<TResult>> func, T1 a, T2 b, T3 c, T4 d, T5 e)
		{
			var task = new TaskCompletionSource<TResult>();
			func(a, b, c, d, e, result => { task.SetResult(result); });
			return task.Task;
		}
	}
}
