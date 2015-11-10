//-----------------------------------------------------------------------
// <copyright file="TaskSynchronizationScope.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Threading.Tasks;

namespace MyToolkit.Utilities
{
    /// <summary>Synchronizes tasks so that they are executed after each other.</summary>
    /// <typeparam name="T">The return type of the task.</typeparam>
    public class TaskSynchronizationScope
    {
        private Task _currentTask;
        private readonly object _lock = new object();

        /// <summary>Executes the given task when the previous task has been completed.</summary>
        /// <param name="task">The task function.</param>
        /// <returns>The task.</returns>
        public Task RunAsync(Func<Task> task)
        {
            return RunAsync<object>(async () =>
            {
                await task();
                return null;
            });
        }

        /// <summary>Executes the given task when the previous task has been completed.</summary>
        /// <param name="task">The task function.</param>
        /// <returns>The task.</returns>
        public Task<T> RunAsync<T>(Func<Task<T>> task)
        {
            lock (_lock)
            {
                if (_currentTask == null)
                {
                    var currentTask = task();
                    _currentTask = currentTask;
                    return currentTask;
                }
                else
                {
                    var source = new TaskCompletionSource<T>();
                    _currentTask.ContinueWith(t =>
                    {
                        var nextTask = task();
                        nextTask.ContinueWith(nt =>
                        {
                            if (nt.IsCompleted)
                                source.SetResult(nt.Result);
                            else if (nt.IsFaulted)
                                source.SetException(nt.Exception);
                            else
                                source.SetCanceled();

                            lock (_lock)
                            {
                                if (_currentTask.Status == TaskStatus.RanToCompletion)
                                    _currentTask = null;
                            }
                        });
                    });
                    _currentTask = source.Task;
                    return source.Task;
                }
            }
        }
    }
}
