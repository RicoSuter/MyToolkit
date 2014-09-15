//-----------------------------------------------------------------------
// <copyright file="EventHelper.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Reflection;

namespace MyToolkit.Utilities
{
    public static class EventHelper
    {
#if !LEGACY
        public static object RegisterEvent(object target, string eventName, Action<object, object> callback)
        {
            var callbackMethodInfo = callback.GetMethodInfo();
            var eventInfo = target.GetType().GetRuntimeEvent(eventName);
            var callbackDelegate = callbackMethodInfo.CreateDelegate(eventInfo.EventHandlerType, callback.Target);
            return eventInfo.AddMethod.Invoke(target, new object[] { callbackDelegate });
        }

        public static object RegisterStaticEvent(Type type, string eventName, Action<object, object> callback)
        {
            var callbackMethodInfo = callback.GetMethodInfo();
            var eventInfo = type.GetRuntimeEvent(eventName);
            var callbackDelegate = callbackMethodInfo.CreateDelegate(eventInfo.EventHandlerType, callback.Target);
            return eventInfo.AddMethod.Invoke(null, new object[] { callbackDelegate });
        }

        public static void UnregisterEvent(object target, string eventName, object token)
        {
            var eventInfo = target.GetType().GetRuntimeEvent(eventName);
            eventInfo.RemoveMethod.Invoke(target, new object[] { token });
        }

        public static void UnregisterStaticEvent(Type type, string eventName, object token)
        {
            var eventInfo = type.GetRuntimeEvent(eventName);
            eventInfo.RemoveMethod.Invoke(null, new object[] { token });
        }
#endif

        /// <summary>Registers a weak event handler which is automatically deregistered after the subscriber 
        /// has been garbage collected (checked on each event call). </summary>
        /// <param name="subscriber"></param>
        /// <param name="add"></param>
        /// <param name="remove"></param>
        /// <param name="converter">The converter: h => (o, e) => h(o, e)</param>
        /// <param name="action"></param>
        public static TDelegate RegisterWeakEvent<TSubscriber, TDelegate, TArgs>(
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

        /// <summary>Registers a weak event handler which is automatically deregistered after the subscriber 
        /// has been garbage collected (checked on each event call). </summary>
        public static EventHandler<TArgs> RegisterWeakEvent<TSubscriber, TArgs>(
            TSubscriber subscriber,
            Action<EventHandler<TArgs>> add,
            Action<EventHandler<TArgs>> remove,
            Action<TSubscriber, object, TArgs> action)
            where TArgs : EventArgs
            where TSubscriber : class
        {
            return RegisterWeakEvent(subscriber, add, remove, h => h, action);
        }
    }
}
