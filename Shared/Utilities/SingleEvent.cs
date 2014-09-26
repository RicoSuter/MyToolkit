using System;
using System.Net;
using System.Windows;
using System.Threading.Tasks;

#if WINRT
using Windows.UI.Xaml;
#endif

namespace MyToolkit.Utilities
{
    [Obsolete("Use EventUtilities instead. 9/25/2014")]
    public class SingleEvent
    {
        internal class SingleEventHandlerContainer<T> where T : EventArgs
        {
            internal EventHandler<T> Handler;
        }

        public static void Register<TObj, T>(TObj sender, Action<TObj, EventHandler<T>> register,
            Action<TObj, EventHandler<T>> deregister, Action<object, T> action) where T : EventArgs
        {
            var wrapper = new SingleEventHandlerContainer<T>();
            wrapper.Handler = delegate(object s, T args)
            {
                deregister((TObj)s, wrapper.Handler);
                action(sender, args);
            };
            register(sender, wrapper.Handler);
        }

        //internal class SingleObjectRoutedEventHandlerContainer
        //{
        //	internal object Handler;
        //}

        //public static void RegisterRouted<TObj, THandler, T>(TObj sender, Action<TObj, THandler> register,
        //	Action<TObj, THandler> deregister, Action<object, T> action) where T : RoutedEventArgs
        //{
        //	var wrapper = new SingleObjectRoutedEventHandlerContainer();
        //	wrapper.Handler = delegate(object s, T args)
        //	{
        //		deregister((TObj)s, wrapper.Handler);
        //		action(sender, args);
        //	};
        //	register(sender, (THandler)wrapper.Handler);
        //}

        internal class SingleRoutedEventHandlerContainer
        {
            internal RoutedEventHandler Handler;
        }

        public static void Register<TObj>(TObj sender, Action<TObj, RoutedEventHandler> register,
            Action<TObj, RoutedEventHandler> deregister, Action<object, RoutedEventArgs> action)
        {
            var wrapper = new SingleRoutedEventHandlerContainer();
            wrapper.Handler = delegate(object s, RoutedEventArgs args)
            {
                deregister((TObj)s, wrapper.Handler);
                action(sender, args);
            };
            register(sender, wrapper.Handler);
        }

#if !WPF

        internal class SingleExceptionRoutedEventHandlerContainer
        {
            internal ExceptionRoutedEventHandler Handler;
        }

        public static void Register<TObj>(TObj sender, Action<TObj, ExceptionRoutedEventHandler> register,
            Action<TObj, ExceptionRoutedEventHandler> deregister, Action<object, RoutedEventArgs> action)
        {
            var wrapper = new SingleExceptionRoutedEventHandlerContainer();
            wrapper.Handler = delegate(object s, ExceptionRoutedEventArgs args)
            {
                deregister((TObj)s, wrapper.Handler);
                action(sender, args);
            };
            register(sender, wrapper.Handler);
        }

        public static Task<RoutedEventArgs> WaitForEventAsync<TObj>(TObj sender, Action<TObj, ExceptionRoutedEventHandler> register,
            Action<TObj, ExceptionRoutedEventHandler> deregister)
        {
            var task = new TaskCompletionSource<RoutedEventArgs>();
            Register(sender, register, deregister, (o, args) => task.SetResult(args));
            return task.Task;
        }

#endif

        public static Task<T> WaitForEventAsync<TObj, T>(TObj sender, Action<TObj, EventHandler<T>> register,
            Action<TObj, EventHandler<T>> deregister) where T : EventArgs
        {
            var task = new TaskCompletionSource<T>();
            Register(sender, register, deregister, (o, args) => task.SetResult(args));
            return task.Task;
        }

        //public static Task WaitForRoutedEventAsync<TObj, THandler, T>(TObj sender, Action<TObj, THandler> register,
        //	Action<TObj, THandler> deregister) where T : RoutedEventArgs
        //{
        //	var task = new TaskCompletionSource<T>();
        //	RegisterRouted<TObj, THandler, T>(sender, register, deregister, (o, args) => task.SetResult(args));
        //	return task.Task;
        //}

        public static Task<RoutedEventArgs> WaitForEventAsync<TObj>(TObj sender, Action<TObj, RoutedEventHandler> register,
            Action<TObj, RoutedEventHandler> deregister)
        {
            var task = new TaskCompletionSource<RoutedEventArgs>();
            Register(sender, register, deregister, (o, args) => task.SetResult(args));
            return task.Task;
        }
    }
}
