using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace MyToolkit.Utilities
{
	public static class TaskHelper
	{
		public static T RunSynchronouslyWithResult<T>(this IAsyncOperation<T> op)
		{
			var task = op.AsTask();
			task.RunSynchronously();
			return task.Result;
		}

		public static T RunSynchronouslyWithResult<T>(this Task<T> task)
		{
			task.RunSynchronously();
			return task.Result;
		}

		public static Task RunCallbackMethod(Action<Action> func)
		{
			var task = new TaskCompletionSource<object>();
			func(() => task.SetResult(new object()));
			return task.Task;
		}

		public static Task<TResult> RunCallbackMethod<TResult>(Action<Action<TResult>> func)
		{
			var task = new TaskCompletionSource<TResult>();
			func(task.SetResult);
			return task.Task;
		}

		public static Task RunCallbackMethod<T1>(Action<T1, Action> func, T1 a)
		{
			var task = new TaskCompletionSource<object>();
			func(a, () => task.SetResult(new object()));
			return task.Task;
		}

		public static Task<TResult> RunCallbackMethod<T1, TResult>(Action<T1, Action<TResult>> func, T1 a)
		{
			var task = new TaskCompletionSource<TResult>();
			func(a, task.SetResult);
			return task.Task;
		}

		public static Task RunCallbackMethod<T1, T2>(Action<T1, T2, Action> func, T1 a, T2 b)
		{
			var task = new TaskCompletionSource<object>();
			func(a, b, () => task.SetResult(new object()));
			return task.Task;
		}

		public static Task<TResult> RunCallbackMethod<T1, T2, TResult>(Action<T1, T2, Action<TResult>> func, T1 a, T2 b)
		{
			var task = new TaskCompletionSource<TResult>();
			func(a, b, task.SetResult);
			return task.Task;
		}

		public static Task RunCallbackMethod<T1, T2, T3>(Action<T1, T2, T3, Action> func, T1 a, T2 b, T3 c)
		{
			var task = new TaskCompletionSource<object>();
			func(a, b, c, () => task.SetResult(new object()));
			return task.Task;
		}

		public static Task<TResult> RunCallbackMethod<T1, T2, T3, TResult>(Action<T1, T2, T3, Action<TResult>> func, T1 a, T2 b, T3 c)
		{
			var task = new TaskCompletionSource<TResult>();
			func(a, b, c, task.SetResult);
			return task.Task;
		}

		public static Task<TResult> RunCallbackMethod<T1, T2, T3, T4, TResult>(Action<T1, T2, T3, T4, Action<TResult>> func, T1 a, T2 b, T3 c, T4 d)
		{
			var task = new TaskCompletionSource<TResult>();
			func(a, b, c, d, task.SetResult);
			return task.Task;
		}

		public static Task<TResult> RunCallbackMethod<T1, T2, T3, T4, T5, TResult>(Action<T1, T2, T3, T4, T5, Action<TResult>> func, T1 a, T2 b, T3 c, T4 d, T5 e)
		{
			var task = new TaskCompletionSource<TResult>();
			func(a, b, c, d, e, task.SetResult);
			return task.Task;
		}
	}
}
