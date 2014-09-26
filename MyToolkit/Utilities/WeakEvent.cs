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

    [Obsolete("Use EventUtilities instead. 9/12/2014")]
    public static class WeakEvent
	{
	    /// <summary>Registers a weak event handler which is automatically deregistered after the subscriber 
	    /// has been garbage collected (checked on each event call). </summary>
	    /// <param name="subscriber"></param>
	    /// <param name="add"></param>
	    /// <param name="remove"></param>
	    /// <param name="converter">The converter: h => (o, e) => h(o, e)</param>
	    /// <param name="action"></param>
        [Obsolete("Use EventUtilities.RegisterWeakEvent instead. 9/12/2014")]
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
	        return EventUtilities.RegisterWeakEvent(subscriber, add, remove, converter, action);
	    }
        
        /// <summary>Registers a weak event handler which is automatically deregistered after the subscriber 
        /// has been garbage collected (checked on each event call). </summary>
        [Obsolete("Use EventUtilities.RegisterWeakEvent instead. 9/12/2014")]
        public static EventHandler<TArgs> Register<TSubscriber, TArgs>(
            TSubscriber subscriber, 
            Action<EventHandler<TArgs>> add, 
            Action<EventHandler<TArgs>> remove,
            Action<TSubscriber, object, TArgs> action)
			where TArgs : EventArgs
            where TSubscriber : class
        {
            return EventUtilities.RegisterWeakEvent(subscriber, add, remove, action);
        }
	}
}