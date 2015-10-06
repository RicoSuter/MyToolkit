//-----------------------------------------------------------------------
// <copyright file="ViewModelBase.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using MyToolkit.Model;
using MyToolkit.Mvvm;

namespace MyToolkit.Mvvm
{
    /// <summary>Provides a base implementation of a view model. </summary>
    [DataContract]
    public class ViewModelBase : ObservableObject
    {
        private int _loadingCounter = 0;
        private List<CancellationTokenSource> _cancellationTokenSources;

        /// <summary>Gets or sets a value indicating whether the view model is currently loading. </summary>
        [XmlIgnore]
        public bool IsLoading
        {
            get { return _loadingCounter > 0; }
            set 
            {
                if (value)
                    _loadingCounter++;
                else if (_loadingCounter > 0)
                    _loadingCounter--;

                RaisePropertyChanged();
            }
        }

        /// <summary>Initializes the view model. Must only be called once per view model instance 
        /// (after the InitializeComponent method of a UserControl). </summary>
        public virtual void Initialize()
        {
            // Must be empty
        }

        /// <summary>Gets a value indicating whether the view model has been loaded. </summary>
        [XmlIgnore]
        public bool IsViewLoaded { get; private set; }

        /// <summary>Registers a <see cref="CancellationTokenSource"/> which will be cancelled when cleaning up the view model. </summary>
        /// <param name="cancellationTokenSource"></param>
        public void RegisterCancellationTokenSource(CancellationTokenSource cancellationTokenSource)
        {
            if (_cancellationTokenSources == null)
                _cancellationTokenSources = new List<CancellationTokenSource>();

            _cancellationTokenSources.Add(cancellationTokenSource);
        }

        /// <summary>Creates a <see cref="CancellationTokenSource"/> and registers it if not disabled. </summary>
        public CancellationTokenSource CreateCancellationTokenSource(bool registerToken = true)
        {
            var token = new CancellationTokenSource();
            if (registerToken)
                RegisterCancellationTokenSource(token);
            return token;
        }
        
        /// <summary>Runs a task and correctly updates the <see cref="IsLoading"/> property, 
        /// handles exceptions in the <see cref="HandleException"/> method 
        /// and automatically creates and registers a cancellation token source. </summary>
        /// <param name="task">The task to run. </param>
        /// <returns>The awaitable task. </returns>
        public async Task<TResult> RunTaskAsync<TResult>(Func<CancellationToken, Task<TResult>> task)
        {
            TResult result = default(TResult);
            var tokenSource = CreateCancellationTokenSource();
            try
            {
                IsLoading = true;
                result = await task(tokenSource.Token);
                IsLoading = false;
            }
            catch (OperationCanceledException)
            {
                IsLoading = false;
            }
            catch (Exception exception)
            {
                IsLoading = false;
                HandleException(exception);
            }
            DeregisterCancellationTokenSource(tokenSource);
            return result;
        }

        /// <summary>Runs a task and correctly updates the <see cref="IsLoading"/> property, 
        /// handles exceptions in the <see cref="HandleException"/> method 
        /// and automatically creates and registers a cancellation token source. </summary>
        /// <param name="task">The task to run. </param>
        /// <returns>The awaitable task. </returns>
        public Task RunTaskAsync(Func<CancellationToken, Task> task)
        {
            return RunTaskAsync(async token =>
            {
                await task(token);
                return (object)null;
            });
        }

        /// <summary>Runs a task and correctly updates the <see cref="IsLoading"/> property, 
        /// handles exceptions in the <see cref="HandleException"/> method 
        /// and automatically creates and registers a cancellation token source. </summary>
        /// <param name="task">The task to run. </param>
        /// <returns>The awaitable task. </returns>
        public Task RunTaskAsync(Func<Task> task)
        {
            return RunTaskAsync(async token =>
            {
                await task();
                return (object)null;
            });
        }

        /// <summary>Runs a task and correctly updates the <see cref="IsLoading"/> property, 
        /// handles exceptions in the <see cref="HandleException"/> method 
        /// and automatically creates and registers a cancellation token source. </summary>
        /// <param name="task">The task to run. </param>
        /// <returns>The awaitable task. </returns>
        public Task<TResult> RunTaskAsync<TResult>(Func<Task<TResult>> task)
        {
            return RunTaskAsync(async token => await task());
        }

        /// <summary>Runs a task and correctly updates the <see cref="IsLoading"/> property, 
        /// handles exceptions in the <see cref="HandleException"/> method 
        /// and automatically creates and registers a cancellation token source. </summary>
        /// <param name="task">The task to run. </param>
        /// <returns>The awaitable task. </returns>
        public async Task<TResult> RunTaskAsync<TResult>(Task<TResult> task)
        {
            TResult result = default(TResult);
            try
            {
                IsLoading = true;
                result = await task;
                IsLoading = false;
            }
            catch (OperationCanceledException)
            {
                IsLoading = false;
            }
            catch (Exception exception)
            {
                IsLoading = false;
                HandleException(exception);
            }
            return result;
        }

        /// <summary>Asynchronously runs an action and correctly updates the <see cref="IsLoading"/> property, 
        /// handles exceptions in the <see cref="HandleException"/> method 
        /// and automatically creates and registers a cancellation token source. </summary>
        /// <param name="task">The task to run. </param>
        /// <returns>The awaitable task. </returns>
        public Task RunTaskAsync(Task task)
        {
            return RunTaskAsync(async () =>
            {
                await task;
                return (object)null;
            });
        }

        /// <summary>Asynchronously runs an action and correctly updates the <see cref="IsLoading"/> property, 
        /// handles exceptions in the <see cref="HandleException"/> method 
        /// and automatically creates and registers a cancellation token source. </summary>
        /// <param name="action">The action to run. </param>
        /// <returns>The awaitable task. </returns>
        public Task RunTaskAsync(Action action)
        {
            return RunTaskAsync(
#if LEGACY
                Task.Factory.StartNew(action)
#else
                Task.Run(action)
#endif
            );
        }

        /// <summary>Asynchronously runs an action and correctly updates the <see cref="IsLoading"/> property, 
        /// handles exceptions in the <see cref="HandleException"/> method 
        /// and automatically creates and registers a cancellation token source. </summary>
        /// <param name="action">The action to run. </param>
        /// <returns>The awaitable task. </returns>
        public async Task<T> RunTaskAsync<T>(Func<T> action)
        {
            return await RunTaskAsync(
#if LEGACY
                Task.Factory.StartNew(action)
#else
                Task.Run(action)
#endif
            );
        }

        /// <summary>Handles an exception which occured in the <c>RunTaskAsync</c> method. </summary>
        /// <param name="exception">The exception to handle. </param>
        public virtual void HandleException(Exception exception)
        {
            throw new NotImplementedException("An exception occured in RunTaskAsync. Override ViewModelBase.HandleException to handle this exception. ", exception);
        }

        /// <summary>Disposes and deregisters a <see cref="CancellationTokenSource"/>. 
        /// Should be called when the task has finished cleaning up the view model. </summary>
        /// <param name="cancellationTokenSource"></param>
        public void DeregisterCancellationTokenSource(CancellationTokenSource cancellationTokenSource)
        {
            try             
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
            }
            catch { }
            _cancellationTokenSources.Remove(cancellationTokenSource);
        }

        /// <summary>Initializes the view model (should be called in the view's Loaded event). </summary>
        public void CallOnLoaded()
        {
            if (!IsViewLoaded)
            {
                OnLoaded();
                IsViewLoaded = true; 
            }
        }

        /// <summary>Cleans up the view model (should be called in the view's Unloaded event). </summary>
        public void CallOnUnloaded()
        {
            if (IsViewLoaded)
            {
                OnUnloaded();
                IsViewLoaded = false;
            }

            CancelAllRunningTasks();
        }

        private void CancelAllRunningTasks()
        {
            if (_cancellationTokenSources != null)
            {
                foreach (var cancellationTokenSource in _cancellationTokenSources.ToArray())
                    DeregisterCancellationTokenSource(cancellationTokenSource);
            }
        }

        /// <summary>Implementation of the initialization method. 
        /// If the view model is already initialized the method is not called again by the Initialize method. </summary>
        protected virtual void OnLoaded()
        {
            // Must be empty
        }

        /// <summary>Implementation of the clean up method. 
        /// If the view model is already cleaned up the method is not called again by the Cleanup method. </summary>
        protected virtual void OnUnloaded()
        {
            // Must be empty
        }
    }
}

namespace MyToolkit.MVVM
{
    /// <summary>Provides a base implementation of a view model. </summary>
    [Obsolete("Use MyToolkit.Mvvm.ViewModelBase instead. 5/17/2014")]
    public class BaseViewModel : ViewModelBase
    {
        
    }
}
