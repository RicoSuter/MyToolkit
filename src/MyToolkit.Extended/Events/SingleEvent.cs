//-----------------------------------------------------------------------
// <copyright file="SingleEvent.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Threading.Tasks;

#if WINRT
using Windows.UI.Xaml;
#else
using System.Windows;
#endif

namespace MyToolkit.Events
{
    /// <summary>Provides methods to register self-deregistering event callbacks. </summary>
    public class SingleEvent
    {
        /// <summary>Asynchronously wait for an occurrence of the given event. </summary>
        /// <typeparam name="TEventSource">The type of the event source. </typeparam>
        /// <typeparam name="TEventArgs">The type of the event args. </typeparam>
        /// <param name="source">The source object. </param>
        /// <param name="register">The event registration action. </param>
        /// <param name="deregister">The event deregistration action. </param>
        /// <returns>The task. </returns>
        public static Task<TEventArgs> WaitForEventAsync<TEventSource, TEventArgs>(TEventSource source, Action<TEventSource, EventHandler<TEventArgs>> register,
            Action<TEventSource, EventHandler<TEventArgs>> deregister)
            where TEventArgs : EventArgs
        {
            var task = new TaskCompletionSource<TEventArgs>();
            RegisterEvent(source, register, deregister, (o, args) => task.SetResult(args));
            return task.Task;
        }

        /// <summary>Asynchronously wait for an occurrence of the given event. </summary>
        /// <typeparam name="TEventSource">The type of the event source. </typeparam>
        /// <param name="source">The source object. </param>
        /// <param name="register">The event registration action. </param>
        /// <param name="deregister">The event deregistration action. </param>
        /// <returns>The task. </returns>
        public static Task WaitForRoutedEventAsync<TEventSource>(TEventSource source,
            Action<TEventSource, RoutedEventHandler> register,
            Action<TEventSource, RoutedEventHandler> deregister)
        {
            var task = new TaskCompletionSource<RoutedEventArgs>();
            RegisterRoutedEvent(source, register, deregister, (o, args) => task.SetResult(args));
            return task.Task;
        }

        /// <summary>Asynchronously wait for an occurrence of the given event. </summary>
        /// <typeparam name="TEventSource">The type of the event source. </typeparam>
        /// <param name="source">The source object. </param>
        /// <param name="register">The event registration action. </param>
        /// <param name="deregister">The event deregistration action. </param>
        /// <returns>The task. </returns>
        public static Task<RoutedEventArgs> WaitForEventAsync<TEventSource>(TEventSource source, Action<TEventSource, RoutedEventHandler> register,
            Action<TEventSource, RoutedEventHandler> deregister)
        {
            var task = new TaskCompletionSource<RoutedEventArgs>();
            RegisterEvent(source, register, deregister, (o, args) => task.SetResult(args));
            return task.Task;
        }

        /// <summary>Registers an event callback which is called once and then automatically deregistered. </summary>
        /// <typeparam name="TEventSource">The type of the event source. </typeparam>
        /// <typeparam name="TEventArgs">The type of the event args. </typeparam>
        /// <param name="source">The source object. </param>
        /// <param name="register">The event registration action. </param>
        /// <param name="deregister">The event deregistration action. </param>
        /// <param name="handler">The event handler. </param>
        /// <returns>The task. </returns>
        public static void RegisterEvent<TEventSource, TEventArgs>(TEventSource source, Action<TEventSource, EventHandler<TEventArgs>> register,
            Action<TEventSource, EventHandler<TEventArgs>> deregister, Action<object, TEventArgs> handler)
            where TEventArgs : EventArgs
        {
            var wrapper = new SingleEventHandlerContainer<TEventArgs>();
            wrapper.Handler = delegate(object s, TEventArgs args)
            {
                deregister((TEventSource)s, wrapper.Handler);
                handler(source, args);
            };
            register(source, wrapper.Handler);
        }

        /// <summary>Registers an event callback which is called once and then automatically deregistered. </summary>
        /// <typeparam name="TEventSource">The type of the event source. </typeparam>
        /// <param name="source">The source object. </param>
        /// <param name="register">The event registration action. </param>
        /// <param name="deregister">The event deregistration action. </param>
        /// <param name="handler">The event handler. </param>
        /// <returns>The task. </returns>
        public static void RegisterRoutedEvent<TEventSource>(TEventSource source, Action<TEventSource, RoutedEventHandler> register,
            Action<TEventSource, RoutedEventHandler> deregister, Action<object, RoutedEventArgs> handler)
        {
            var wrapper = new SingleRoutedEventHandlerContainer();
            wrapper.Handler = delegate(object s, RoutedEventArgs args)
            {
                deregister((TEventSource)s, wrapper.Handler);
                handler(source, args);
            };
            register(source, wrapper.Handler);
        }

        /// <summary>Registers an event callback which is called once and then automatically deregistered. </summary>
        /// <typeparam name="TEventSource">The type of the event source. </typeparam>
        /// <param name="source">The source object. </param>
        /// <param name="register">The event registration action. </param>
        /// <param name="deregister">The event deregistration action. </param>
        /// <param name="handler">The event handler. </param>
        /// <returns>The task. </returns>
        public static void RegisterEvent<TEventSource>(TEventSource source, Action<TEventSource, RoutedEventHandler> register,
            Action<TEventSource, RoutedEventHandler> deregister, Action<object, RoutedEventArgs> handler)
        {
            var wrapper = new SingleRoutedEventHandlerContainer();
            wrapper.Handler = delegate(object s, RoutedEventArgs args)
            {
                deregister((TEventSource)s, wrapper.Handler);
                handler(source, args);
            };
            register(source, wrapper.Handler);
        }

#if !WPF

        /// <summary>Registers an event callback which is called once and then automatically deregistered. </summary>
        /// <typeparam name="TEventSource">The type of the event source. </typeparam>
        /// <param name="source">The source object. </param>
        /// <param name="register">The event registration action. </param>
        /// <param name="deregister">The event deregistration action. </param>
        /// <param name="handler">The event handler. </param>
        /// <returns>The task. </returns>
        public static void RegisterEvent<TEventSource>(TEventSource source, Action<TEventSource, ExceptionRoutedEventHandler> register,
            Action<TEventSource, ExceptionRoutedEventHandler> deregister, Action<object, RoutedEventArgs> handler)
        {
            var wrapper = new SingleExceptionRoutedEventHandlerContainer();
            wrapper.Handler = delegate(object s, ExceptionRoutedEventArgs args)
            {
                deregister((TEventSource)s, wrapper.Handler);
                handler(source, args);
            };
            register(source, wrapper.Handler);
        }

        /// <summary>Asynchronously wait for an occurrence of the given event. </summary>
        /// <typeparam name="TEventSource">The type of the event source. </typeparam>
        /// <param name="source">The source object. </param>
        /// <param name="register">The event registration action. </param>
        /// <param name="deregister">The event deregistration action. </param>
        /// <returns>The task. </returns>
        public static Task<RoutedEventArgs> WaitForEventAsync<TEventSource>(TEventSource source, 
            Action<TEventSource, ExceptionRoutedEventHandler> register,
            Action<TEventSource, ExceptionRoutedEventHandler> deregister)
        {
            var task = new TaskCompletionSource<RoutedEventArgs>();
            RegisterEvent(source, register, deregister, (o, args) => task.SetResult(args));
            return task.Task;
        }

#endif
    }

    internal class SingleRoutedEventHandlerContainer
    {
        internal RoutedEventHandler Handler;
    }

    internal class SingleEventHandlerContainer<T> where T : EventArgs
    {
        internal EventHandler<T> Handler;
    }

#if !WPF
    
    internal class SingleExceptionRoutedEventHandlerContainer
    {
        internal ExceptionRoutedEventHandler Handler;
    }

#endif
}