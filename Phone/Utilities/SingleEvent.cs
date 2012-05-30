using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace MyToolkit.Utilities
{
	public class SingleEvent
	{
		internal class SingleEventContainer<T> where T : EventArgs
		{
			internal EventHandler<T> Handler;
		}

		public static void Register<TObj, T>(TObj sender, Action<TObj, EventHandler<T>> register,
			Action<TObj, EventHandler<T>> unregister, Action<object, T> action) where T : EventArgs
		{
			var wrapper = new SingleEventContainer<T>();
			wrapper.Handler = delegate(object s, T args)
			{
				unregister((TObj)s, wrapper.Handler);
				action(sender, args);
			};
			register(sender, wrapper.Handler);
		}
	}
}
