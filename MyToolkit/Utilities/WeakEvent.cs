//-----------------------------------------------------------------------
// <copyright file="WeakEvent.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;

namespace MyToolkit.Utilities
{
	// see http://stackoverflow.com/questions/1747235/weak-event-handler-model-for-use-with-lambdas

	public static class WeakEvent
	{
	    /// <summary>
	    /// Registers a weak event handler which is automatically deregistered after the subscriber 
	    /// has been garbage collected (checked on each event call). 
	    /// </summary>
	    /// <param name="subscriber"></param>
	    /// <param name="add"></param>
	    /// <param name="remove"></param>
	    /// <param name="converter">The converter: h => (o, e) => h(o, e)</param>
	    /// <param name="action"></param>
        public static TDelegate Register<TSubscriber, TDelegate, TArgs>(
            TSubscriber subscriber, 
            Action<TDelegate> add, 
            Action<TDelegate> remove, 
            Func<EventHandler<TArgs>, TDelegate> converter,
            Action<TSubscriber, object, TArgs> action)
            where TArgs : EventArgs
			where TDelegate : class
            where TSubscriber : class
		{
			var weakReference = new WeakReference(subscriber);
			TDelegate handler = null;
			handler = converter(
				(s, e) =>
				{
                    var strongReference = weakReference.Target as TSubscriber;
					if (strongReference != null)
                        action(strongReference, s, e);
					else
					{
						remove(handler);
						handler = null;
					}
				});
			add(handler);
			return handler; 
		}
        
        /// <summary>
        /// Registers a weak event handler which is automatically deregistered after the subscriber 
        /// has been garbage collected (checked on each event call). 
        /// </summary>
        public static EventHandler<TArgs> Register<TSubscriber, TArgs>(
            TSubscriber subscriber, 
            Action<EventHandler<TArgs>> add, 
            Action<EventHandler<TArgs>> remove,
            Action<TSubscriber, object, TArgs> action)
			where TArgs : EventArgs
            where TSubscriber : class
		{
			return Register(subscriber, add, remove, h => h, action);
		}
	}
}