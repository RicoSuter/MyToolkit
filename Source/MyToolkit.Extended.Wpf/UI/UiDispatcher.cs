using System;
using System.Windows.Threading;
using MyToolkit.Mvvm;

namespace MyToolkit.UI
{
    // TODO: Add to other projects
    public class UiDispatcher : IDispatcher
    {
        private readonly Dispatcher _dispatcher;

        public UiDispatcher(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        /// <summary>
        /// Invokes an action on the dispatcher thread. 
        /// </summary>
        /// <param name="action">The action. </param>
        public void InvokeAsync(Action action)
        {
            _dispatcher.InvokeAsync(action);
        }
    }
}
