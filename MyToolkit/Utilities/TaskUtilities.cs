//-----------------------------------------------------------------------
// <copyright file="TaskUtilities.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Threading.Tasks;

namespace MyToolkit.Utilities
{
    /// <summary>Provides task helper methods. </summary>
	public static class TaskUtilities
	{
		public static T RunSynchronouslyWithResult<T>(this Task<T> task)
		{
			if (task.IsCompleted)
				return task.Result;
			task.RunSynchronously();
			return task.Result;
		}

		public static Task RunCallbackMethodAsync(Action<Action> func)
		{
			var task = new TaskCompletionSource<object>();
			func(() => task.SetResult(new object()));
			return task.Task;
		}

		public static Task<TResult> RunCallbackMethodAsync<TResult>(Action<Action<TResult>> func)
		{
			var task = new TaskCompletionSource<TResult>();
			func(task.SetResult);
			return task.Task;
		}

		public static Task RunCallbackMethodAsync<T1>(Action<T1, Action> func, T1 a)
		{
			var task = new TaskCompletionSource<object>();
			func(a, () => task.SetResult(new object()));
			return task.Task;
		}

		public static Task<TResult> RunCallbackMethodAsync<T1, TResult>(Action<T1, Action<TResult>> func, T1 a)
		{
			var task = new TaskCompletionSource<TResult>();
			func(a, task.SetResult);
			return task.Task;
		}

		public static Task RunCallbackMethodAsync<T1, T2>(Action<T1, T2, Action> func, T1 a, T2 b)
		{
			var task = new TaskCompletionSource<object>();
			func(a, b, () => task.SetResult(new object()));
			return task.Task;
		}

		public static Task<TResult> RunCallbackMethodAsync<T1, T2, TResult>(Action<T1, T2, Action<TResult>> func, T1 a, T2 b)
		{
			var task = new TaskCompletionSource<TResult>();
			func(a, b, task.SetResult);
			return task.Task;
		}

		public static Task RunCallbackMethodAsync<T1, T2, T3>(Action<T1, T2, T3, Action> func, T1 a, T2 b, T3 c)
		{
			var task = new TaskCompletionSource<object>();
			func(a, b, c, () => task.SetResult(new object()));
			return task.Task;
		}

		public static Task<TResult> RunCallbackMethodAsync<T1, T2, T3, TResult>(Action<T1, T2, T3, Action<TResult>> func, T1 a, T2 b, T3 c)
		{
			var task = new TaskCompletionSource<TResult>();
			func(a, b, c, task.SetResult);
			return task.Task;
		}
	}
}
