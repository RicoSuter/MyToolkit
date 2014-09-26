//-----------------------------------------------------------------------
// <copyright file="EventUtilities.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MyToolkit.Utilities
{
    /// <summary>Provides methods for event management. </summary>
    public class EventUtilities
    {
        private static List<WeakEvent> _registeredWeakEvents = null;

        internal static List<WeakEvent> RegisteredWeakEvents
        {
            get
            {
                if (_registeredWeakEvents == null)
                {
                    lock (typeof(EventUtilities))
                    {
                        if (_registeredWeakEvents == null)
                            _registeredWeakEvents = new List<WeakEvent>();
                    }
                }
                return _registeredWeakEvents;
            }
        }

        /// <summary>Registers an event callback which is deregistered after its first occurrence. </summary>
        /// <typeparam name="TEventSource">The type of the event source. </typeparam>
        /// <typeparam name="TEventArgs">The type of the event args. </typeparam>
        /// <param name="source">The source object. </param>
        /// <param name="register">The action to register the event. </param>
        /// <param name="deregister">The action to deregister the event. </param>
        /// <param name="handler">The handler. </param>
        public static void RegisterSingleEvent<TEventSource, TEventArgs>(
            TEventSource source, 
            Action<TEventSource, EventHandler<TEventArgs>> register,
            Action<TEventSource, EventHandler<TEventArgs>> deregister, 
            EventHandler<TEventArgs> handler) where TEventArgs : EventArgs
        {
            var wrapper = new SingleEventHandlerContainer<TEventArgs>();
            wrapper.Handler = delegate(object s, TEventArgs args)
            {
                deregister((TEventSource)s, wrapper.Handler);
                handler(source, args);
            };
            register(source, wrapper.Handler);
        }

        /// <summary>Waits for an occurrence of an event. </summary>
        /// <typeparam name="TEventSource">The type of the event source. </typeparam>
        /// <typeparam name="TEventArgs">The type of the event args. </typeparam>
        /// <param name="source">The source object. </param>
        /// <param name="register">The action to register the event. </param>
        /// <param name="deregister">The action to deregister the event. </param>
        /// <returns>The task. </returns>
        public static Task<TEventArgs> WaitForEventAsync<TEventSource, TEventArgs>(
            TEventSource source, 
            Action<TEventSource, EventHandler<TEventArgs>> register, 
            Action<TEventSource, EventHandler<TEventArgs>> deregister) 
            where TEventArgs : EventArgs
        {
            var task = new TaskCompletionSource<TEventArgs>();
            RegisterSingleEvent(source, register, deregister, (o, args) => task.SetResult(args));
            return task.Task;
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
            Func<EventHandler<TArgs>, EventHandler<TArgs>> converter = h => h;
            var weakReference = new WeakReference(subscriber);
            EventHandler<TArgs> handler = null;
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
        /// <param name="converter">The converter: h => (o, e) => h(o, e)</param>
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

#if !LEGACY

        /// <summary>Registers an event on the given target object. </summary>
        /// <param name="target">The target object. </param>
        /// <param name="eventName">The event name. </param>
        /// <param name="callback">The callback. </param>
        /// <returns>The registration token to deregister the event. </returns>
        public static object RegisterEvent(object target, string eventName, Action<object, object> callback)
        {
            var callbackMethodInfo = callback.GetMethodInfo();
            var eventInfo = target.GetType().GetRuntimeEvent(eventName);
            var callbackDelegate = callbackMethodInfo.CreateDelegate(eventInfo.EventHandlerType, callback.Target);
            return eventInfo.AddMethod.Invoke(target, new object[] { callbackDelegate });
        }

        /// <summary>Registers a static event on the given target object. </summary>
        /// <param name="type">The target type. </param>
        /// <param name="eventName">The event name. </param>
        /// <param name="callback">The callback. </param>
        /// <returns>The registration token to deregister the event. </returns>
        public static object RegisterStaticEvent(Type type, string eventName, Action<object, object> callback)
        {
            var callbackMethodInfo = callback.GetMethodInfo();
            var eventInfo = type.GetRuntimeEvent(eventName);
            var callbackDelegate = callbackMethodInfo.CreateDelegate(eventInfo.EventHandlerType, callback.Target);
            return eventInfo.AddMethod.Invoke(null, new object[] { callbackDelegate });
        }

        /// <summary>Deregisters an event from the target object. </summary>
        /// <param name="target">The target object. </param>
        /// <param name="eventName">The event name. </param>
        /// <param name="token">The registration token. </param>
        public static void DeregisterEvent(object target, string eventName, object token)
        {
            var eventInfo = target.GetType().GetRuntimeEvent(eventName);
            eventInfo.RemoveMethod.Invoke(target, new object[] { token });
        }

        /// <summary>Deregisters a static event from the target type. </summary>
        /// <param name="type">The target type. </param>
        /// <param name="eventName">The event name. </param>
        /// <param name="token">The registration token. </param>
        public static void DeregisterStaticEvent(Type type, string eventName, object token)
        {
            var eventInfo = type.GetRuntimeEvent(eventName);
            eventInfo.RemoveMethod.Invoke(null, new object[] { token });
        }

        /// <summary>Adds a weak event handler to the given source object. </summary>
        /// <typeparam name="TEventSource">The type of the source object.</typeparam>
        /// <typeparam name="TEventArgs">The type of the event args.</typeparam>
        /// <param name="source">The source object to register the event on. </param>
        /// <param name="eventName">The event name to create the registration for.</param>
        /// <param name="handler">The delegate that handles the event.</param>
        public static void RegisterWeakEvent<TEventSource, TEventArgs>(TEventSource source, string eventName, EventHandler<TEventArgs> handler)
        {
            var eventInfo = typeof(TEventSource).GetRuntimeEvent(eventName);
            RegisteredWeakEvents.Add(new WeakEvent(source, eventInfo, handler));
        }

        /// <summary>Adds a static weak event handler to a static event. </summary>
        /// <typeparam name="TEventArgs">The type of the event args.</typeparam>
        /// <param name="sourceType">The type of the class that contains the static event. </param>
        /// <param name="eventName">The event name to create the registration for.</param>
        /// <param name="handler">The delegate that handles the event.</param>
        public static void RegisterStaticWeakEvent<TEventArgs>(Type sourceType, string eventName, EventHandler<TEventArgs> handler)
        {
            var eventInfo = sourceType.GetRuntimeEvent(eventName);
            RegisteredWeakEvents.Add(new WeakEvent(null, eventInfo, handler));
        }

        /// <summary>Adds a static weak event handler to a static event. </summary>
        /// <typeparam name="TEventArgs">The type of the event args.</typeparam>
        /// <typeparam name="TEventSource">The type of the class that contains the static event. </typeparam>
        /// <param name="eventName">The event name to create the registration for.</param>
        /// <param name="handler">The delegate that handles the event.</param>
        public static void RegisterStaticWeakEvent<TEventSource, TEventArgs>(string eventName, EventHandler<TEventArgs> handler)
        {
            var eventInfo = typeof(TEventSource).GetRuntimeEvent(eventName);
            RegisteredWeakEvents.Add(new WeakEvent(null, eventInfo, handler));
        }

        /// <summary>Removes a weak event registration from the given source object.</summary>
        /// <typeparam name="TEventSource">The type of the source object.</typeparam>
        /// <param name="source">The source object to register the event from. </param>
        /// <param name="eventName">The event name to remove the registration from.</param>
        /// <param name="handler">The handler to remove.</param>
        /// <returns>True if the event registration could be found and was removed. </returns>
        public static bool DeregisterWeakEvent<TEventSource>(TEventSource source, string eventName, Delegate handler)
        {
            var eventInfo = typeof(TEventSource).GetRuntimeEvent(eventName);
            return DeregisterWeakEvent(source, handler, eventInfo);
        }

        /// <summary>Removes a static weak event registration from a static event.</summary>
        /// <param name="sourceType">The type of the class that contains the static event. </param>
        /// <param name="eventName">The event name to remove the registration from.</param>
        /// <param name="handler">The handler to remove. </param>
        /// <returns>True if the event registration could be found and was removed. </returns>
        public static bool DeregisterStaticWeakEvent(Type sourceType, string eventName, Delegate handler)
        {
            var eventInfo = sourceType.GetRuntimeEvent(eventName);
            return DeregisterWeakEvent(null, handler, eventInfo);
        }

        private static bool DeregisterWeakEvent(object source, Delegate handler, EventInfo eventInfo)
        {
            var weakEvent = RegisteredWeakEvents.FirstOrDefault(e => e.Matches(source, eventInfo, handler));
            if (weakEvent != null)
                weakEvent.DeregisterEvent();
            return weakEvent != null;
        }

        internal class WeakEvent
        {
            private static readonly MethodInfo _onEventCalledInfo =
              typeof(WeakEvent).GetTypeInfo().GetDeclaredMethod("OnEventCalled");

            private EventInfo _eventInfo;
            private object _eventHandler;

            private readonly object _source;

            private readonly MethodInfo _handlerMethod;
            private readonly WeakReference<object> _handlerTarget;

            public WeakEvent(object source, EventInfo eventInfo, Delegate handler)
            {
                _source = source;
                _eventInfo = eventInfo;

                _handlerMethod = handler.GetMethodInfo();
                _handlerTarget = new WeakReference<object>(handler.Target);

                var eventHandler = CreateEventHandler();
                _eventHandler = eventInfo.AddMethod.Invoke(source, new object[] { eventHandler });

                if (_eventHandler == null)
                    _eventHandler = eventHandler;
            }

            public bool Matches(object source, EventInfo eventInfo, Delegate handler)
            {
                if (source == _source && Equals(eventInfo, _eventInfo))
                {
                    object target;
                    if (_handlerTarget.TryGetTarget(out target))
                        return handler.Target == target && Equals(handler.GetMethodInfo(), _handlerMethod);
                }

                return false;
            }

            public void DeregisterEvent()
            {
                if (_eventInfo != null)
                {
                    EventUtilities.RegisteredWeakEvents.Remove(this);

                    _eventInfo.RemoveMethod.Invoke(_source, new object[] { _eventHandler });

                    _eventHandler = null;
                    _eventInfo = null;
                }
            }

            public void OnEventCalled<T>(object sender, T args)
            {
                object instance;
                if (_handlerTarget.TryGetTarget(out instance))
                    _handlerMethod.Invoke(instance, new object[] { sender, args });
                else
                    DeregisterEvent();
            }

            private object CreateEventHandler()
            {
                Type eventType = _eventInfo.EventHandlerType;
                ParameterInfo[] parameters = eventType.GetTypeInfo()
                  .GetDeclaredMethod("Invoke")
                  .GetParameters();

                return _onEventCalledInfo
                  .MakeGenericMethod(parameters[1].ParameterType)
                  .CreateDelegate(eventType, this);
            }
        }

#else

        /// <summary>Registers an event on the given target object. </summary>
        /// <param name="target">The target object. </param>
        /// <param name="eventName">The event name. </param>
        /// <param name="callback">The callback. </param>
        /// <returns>The registration token to deregister the event. </returns>
        public static object RegisterEvent(object target, string eventName, Action<object, object> callback)
        {
            var callbackMethodInfo = callback.Method;
            var eventInfo = target.GetType().GetEvent(eventName);
            var callbackDelegate = Delegate.CreateDelegate(eventInfo.EventHandlerType, callback.Target, callbackMethodInfo);
            return eventInfo.GetAddMethod().Invoke(target, new object[] { callbackDelegate });
        }

        /// <summary>Registers a static event on the given target object. </summary>
        /// <param name="type">The target type. </param>
        /// <param name="eventName">The event name. </param>
        /// <param name="callback">The callback. </param>
        /// <returns>The registration token to deregister the event. </returns>
        public static object RegisterStaticEvent(Type type, string eventName, Action<object, object> callback)
        {
            var callbackMethodInfo = callback.Method;
            var eventInfo = type.GetEvent(eventName);
            var callbackDelegate = Delegate.CreateDelegate(eventInfo.EventHandlerType, callback.Target, callbackMethodInfo);
            return eventInfo.GetAddMethod().Invoke(null, new object[] { callbackDelegate });
        }

        /// <summary>Deregisters an event from the target object. </summary>
        /// <param name="target">The target object. </param>
        /// <param name="eventName">The event name. </param>
        /// <param name="token">The registration token. </param>
        public static void DeregisterEvent(object target, string eventName, object token)
        {
            var eventInfo = target.GetType().GetEvent(eventName);
            eventInfo.GetRemoveMethod().Invoke(target, new object[] { token });
        }

        /// <summary>Deregisters a static event from the target type. </summary>
        /// <param name="type">The target type. </param>
        /// <param name="eventName">The event name. </param>
        /// <param name="token">The registration token. </param>
        public static void DeregisterStaticEvent(Type type, string eventName, object token)
        {
            var eventInfo = type.GetEvent(eventName);
            eventInfo.GetRemoveMethod().Invoke(null, new object[] { token });
        }

        /// <summary>Adds a weak event handler to the given source object. </summary>
        /// <typeparam name="TEventSource">The type of the source object.</typeparam>
        /// <typeparam name="TEventArgs">The type of the event args.</typeparam>
        /// <param name="source">The source object to register the event on. </param>
        /// <param name="eventName">The event name to create the registration for.</param>
        /// <param name="handler">The delegate that handles the event.</param>
        public static void RegisterWeakEvent<TEventSource, TEventArgs>(TEventSource source, string eventName, EventHandler<TEventArgs> handler)
            where TEventArgs : EventArgs
        {
            var eventInfo = typeof(TEventSource).GetEvent(eventName);
            RegisteredWeakEvents.Add(new WeakEvent(source, eventInfo, handler));
        }

        /// <summary>Adds a static weak event handler to a static event. </summary>
        /// <typeparam name="TEventArgs">The type of the event args.</typeparam>
        /// <param name="sourceType">The type of the class that contains the static event. </param>
        /// <param name="eventName">The event name to create the registration for.</param>
        /// <param name="handler">The delegate that handles the event.</param>
        public static void RegisterStaticWeakEvent<TEventArgs>(Type sourceType, string eventName, EventHandler<TEventArgs> handler)
            where TEventArgs : EventArgs
        {
            var eventInfo = sourceType.GetEvent(eventName);
            RegisteredWeakEvents.Add(new WeakEvent(null, eventInfo, handler));
        }

        /// <summary>Adds a static weak event handler to a static event. </summary>
        /// <typeparam name="TEventArgs">The type of the event args.</typeparam>
        /// <typeparam name="TEventSource">The type of the class that contains the static event. </typeparam>
        /// <param name="eventName">The event name to create the registration for.</param>
        /// <param name="handler">The delegate that handles the event.</param>
        public static void RegisterStaticWeakEvent<TEventSource, TEventArgs>(string eventName, EventHandler<TEventArgs> handler)
            where TEventArgs : EventArgs
        {
            var eventInfo = typeof(TEventSource).GetEvent(eventName);
            RegisteredWeakEvents.Add(new WeakEvent(null, eventInfo, handler));
        }

        /// <summary>Removes a weak event registration from the given source object.</summary>
        /// <typeparam name="TEventSource">The type of the source object.</typeparam>
        /// <param name="source">The source object to register the event from. </param>
        /// <param name="eventName">The event name to remove the registration from.</param>
        /// <param name="handler">The handler to remove.</param>
        /// <returns>True if the event registration could be found and was removed. </returns>
        public static bool DeregisterWeakEvent<TEventSource>(TEventSource source, string eventName, Delegate handler)
        {
            var eventInfo = typeof(TEventSource).GetEvent(eventName);
            return DeregisterWeakEvent(source, handler, eventInfo);
        }

        /// <summary>Removes a static weak event registration from a static event.</summary>
        /// <param name="sourceType">The type of the class that contains the static event. </param>
        /// <param name="eventName">The event name to remove the registration from.</param>
        /// <param name="handler">The handler to remove. </param>
        /// <returns>True if the event registration could be found and was removed. </returns>
        public static bool DeregisterStaticWeakEvent(Type sourceType, string eventName, Delegate handler)
        {
            var eventInfo = sourceType.GetEvent(eventName);
            return DeregisterWeakEvent(null, handler, eventInfo);
        }

        private static bool DeregisterWeakEvent(object source, Delegate handler, EventInfo eventInfo)
        {
            var weakEvent = RegisteredWeakEvents.FirstOrDefault(e => e.Matches(source, eventInfo, handler));
            if (weakEvent != null)
                weakEvent.DeregisterEvent();
            return weakEvent != null;
        }

        internal class WeakEvent
        {
            private static readonly MethodInfo _onEventCalledInfo =
              typeof(WeakEvent).GetMethod("OnEventCalled");

            private EventInfo _eventInfo;
            private object _eventHandler;

            private readonly object _source;

            private readonly MethodInfo _handlerMethod;
            private readonly WeakReference _handlerTarget;

            public WeakEvent(object source, EventInfo eventInfo, Delegate handler)
            {
                _source = source;
                _eventInfo = eventInfo;

                _handlerMethod = handler.Method;
                _handlerTarget = new WeakReference(handler.Target);

                var eventHandler = CreateEventHandler();
                _eventHandler = eventInfo.GetAddMethod().Invoke(source, new object[] { eventHandler });

                if (_eventHandler == null)
                    _eventHandler = eventHandler;
            }

            public bool Matches(object source, EventInfo eventInfo, Delegate handler)
            {
                if (source == _source && Equals(eventInfo, _eventInfo))
                {
                    var target = _handlerTarget.Target;
                    if (target != null)
                        return handler.Target == target && Equals(handler.Method, _handlerMethod);
                }

                return false;
            }

            public void DeregisterEvent()
            {
                if (_eventInfo != null)
                {
                    EventUtilities.RegisteredWeakEvents.Remove(this);

                    _eventInfo.GetRemoveMethod().Invoke(_source, new object[] { _eventHandler });

                    _eventHandler = null;
                    _eventInfo = null;
                }
            }

            public void OnEventCalled<T>(object sender, T args)
            {
                var instance = _handlerTarget.Target;
                if (instance != null)
                    _handlerMethod.Invoke(instance, new object[] { sender, args });
                else
                    DeregisterEvent();
            }

            private object CreateEventHandler()
            {
                Type eventType = _eventInfo.EventHandlerType;
                return Delegate.CreateDelegate(eventType, this, _onEventCalledInfo);
            }
        }

#endif

        internal class SingleEventHandlerContainer<T> 
            where T : EventArgs
        {
            public EventHandler<T> Handler { get; set; }
        }
    }

    [Obsolete("Use EventUtilities instead. 9/25/2014")]
    public class EventHelper : EventUtilities
    {
        // TODO: Make EventUtilities class static
    }
}
