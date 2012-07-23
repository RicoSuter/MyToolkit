using System;
using System.Net;
using System.Windows;

#if METRO
using System.Threading.Tasks;
using Windows.UI.Xaml;
#endif

namespace MyToolkit.Utilities
{
	public class SingleEvent
	{
		internal class SingleEventHandlerContainer<T> where T : EventArgs
		{
			internal EventHandler<T> Handler;
		}

		public static void Register<TObj, T>(TObj sender, Action<TObj, EventHandler<T>> register,
			Action<TObj, EventHandler<T>> unregister, Action<object, T> action) where T : EventArgs
		{
			var wrapper = new SingleEventHandlerContainer<T>();
			wrapper.Handler = delegate(object s, T args)
			{
				unregister((TObj)s, wrapper.Handler);
				action(sender, args);
			};
			register(sender, wrapper.Handler);
		}

		internal class SingleRoutedEventHandlerContainer
		{
			internal RoutedEventHandler Handler;
		}

		public static void Register<TObj>(TObj sender, Action<TObj, RoutedEventHandler> register,
			Action<TObj, RoutedEventHandler> unregister, Action<object, RoutedEventArgs> action)
		{
			var wrapper = new SingleRoutedEventHandlerContainer();
			wrapper.Handler = delegate(object s, RoutedEventArgs args)
			{
				unregister((TObj)s, wrapper.Handler);
				action(sender, args);
			};
			register(sender, wrapper.Handler);
		}

		internal class SingleExceptionRoutedEventHandlerContainer
		{
			internal ExceptionRoutedEventHandler Handler;
		}

		public static void Register<TObj>(TObj sender, Action<TObj, ExceptionRoutedEventHandler> register,
			Action<TObj, ExceptionRoutedEventHandler> unregister, Action<object, RoutedEventArgs> action)
		{
			var wrapper = new SingleExceptionRoutedEventHandlerContainer();
			wrapper.Handler = delegate(object s, ExceptionRoutedEventArgs args)
			{
				unregister((TObj)s, wrapper.Handler);
				action(sender, args);
			};
			register(sender, wrapper.Handler);
		}

#if METRO
		public static Task WaitForEventAsync<TObj, T>(TObj sender, Action<TObj, EventHandler<T>> register,
			Action<TObj, EventHandler<T>> unregister) where T : EventArgs
		{
			var task = new TaskCompletionSource<T>();
			Register(sender, register, unregister, (o, args) => task.SetResult(args));
			return task.Task;
		}
		
		public static Task WaitForEventAsync<TObj>(TObj sender, Action<TObj, RoutedEventHandler> register,
			Action<TObj, RoutedEventHandler> unregister)
		{
			var task = new TaskCompletionSource<RoutedEventArgs>();
			Register(sender, register, unregister, (o, args) => task.SetResult(args));
			return task.Task;
		}
		
		public static Task WaitForEventAsync<TObj>(TObj sender, Action<TObj, ExceptionRoutedEventHandler> register,
			Action<TObj, ExceptionRoutedEventHandler> unregister)
		{
			var task = new TaskCompletionSource<RoutedEventArgs>();
			Register(sender, register, unregister, (o, args) => task.SetResult(args));
			return task.Task;
		}
#endif
	}
}
