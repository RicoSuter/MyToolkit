//-----------------------------------------------------------------------
// <copyright file="Messenger.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyToolkit.Messaging
{
    /// <summary>An instance of the messenger. </summary>
	public class Messenger : IMessenger
    {
		private readonly List<MessageRegistration> _actions = new List<MessageRegistration>();
        private static IMessenger _defaultMessenger; 

        /// <summary>Gets or sets the default messenger. </summary>
        public static IMessenger Default
	    {
	        get
	        {
	            if (_defaultMessenger == null)
	            {
	                lock (typeof (Messenger))
	                {
	                    if (_defaultMessenger == null)
                            _defaultMessenger = new Messenger();
	                }
	            }
	            return _defaultMessenger;
	        }
	        set { _defaultMessenger = value;  }
	    }
		
		/// <summary>Registers an action for the given receiver. WARNING: You have to unregister the action to avoid memory leaks! </summary>
		/// <typeparam name="T">Type of the message. </typeparam>
		/// <param name="receiver">Receiver to use as identifier. </param>
		/// <param name="action">Action to register. </param>
		public void Register<T>(object receiver, Action<T> action)
		{
			_actions.Add(new MessageRegistration { Receiver = receiver, Type = typeof(T), Action = action});
		}

		/// <summary>Registers an action for no receiver. </summary>
		/// <typeparam name="T">Type of the message. </typeparam>
		/// <param name="action">Action to register. </param>
		public void Register<T>(Action<T> action)
		{
			Register(null, action);
		}

		/// <summary>Unregisters all actions with no receiver. </summary>
		public void Unregister()
		{
			Unregister(null);
		}

		/// <summary>Unregisters all actions with the given receiver. </summary>
		/// <param name="receiver"></param>
		public void Unregister(object receiver)
		{
			foreach (var a in _actions.Where(a => a.Receiver == receiver).ToArray())
				_actions.Remove(a);
		}

		/// <summary>Unregisters the specified action. </summary>
		/// <typeparam name="T">Type of the message. </typeparam>
		/// <param name="action">Action to unregister. </param>
		public void Unregister<T>(Action<T> action)
		{
			foreach (var a in _actions.Where(a => (Action<T>)a.Action == action).ToArray())
				_actions.Remove(a);
		}

		/// <summary>Unregisters the specified action. </summary>
		/// <typeparam name="T">Type of the message</typeparam>
		public void Unregister<T>()
		{
			foreach (var a in _actions.Where(a => a.Type == typeof(T)).ToArray())
				_actions.Remove(a);
		}

		/// <summary>Unregisters the specified action. </summary>
		/// <param name="receiver"></param>
		/// <typeparam name="T">Type of the message</typeparam>
		public void Unregister<T>(object receiver)
		{
			foreach (var a in _actions.Where(a => a.Receiver == receiver && a.Type == typeof(T)).ToArray())
				_actions.Remove(a);
		}

		/// <summary>Unregisters an action for the specified receiver. </summary>
		/// <typeparam name="T">Type of the message. </typeparam>
		/// <param name="receiver">The receiver object. </param>
		/// <param name="action">The action to unregister on the receiver. </param>
		public void Unregister<T>(object receiver, Action<T> action)
		{
			foreach (var a in _actions.Where(a => a.Receiver == receiver && (Action<T>)a.Action == action).ToArray())
				_actions.Remove(a);
		}

        /// <summary>Sends a message to the registered receivers. </summary>
        /// <typeparam name="T">Type of the message. </typeparam>
        /// <param name="message">The message to send. </param>
        public virtual T Send<T>(T message)
	    {
            var type = message.GetType();
            foreach (var a in _actions.Where(a => a.Type == type).ToArray())
                ((Delegate)a.Action).DynamicInvoke(message);
            return message;
	    }

        /// <summary>Sends a message to the registered receivers. 
        /// Usage: new TextMessage("Test").Send();
        /// Returns the input message for chaining. </summary>
        /// <param name="message">The message to send. </param>
        public Task SendAsync(CallbackMessage message)
        {
            Send(message);
            return message.Task;
        }

        /// <summary>Usage: new TextMessage("Test").Send();
        /// Returns the input message for chaining. </summary>
        /// <param name="message">The message to send. </param>
        public Task<CallbackMessageResult<T>> SendAsync<T>(CallbackMessage<T> message)
        {
            Send(message);
            return message.Task;
        }

        /// <summary>Sends a message to the registered receivers. 
        /// Usage: new TextMessage("Test").Send();
        /// Returns the input message for chaining. </summary>
        /// <param name="message">The message to send. </param>
        public Task<CallbackMessageResult<TFirst, TSecond>> SendAsync<TFirst, TSecond>(CallbackMessage<TFirst, TSecond> message)
        {
            Send(message);
            return message.Task;
        }

        /// <summary>Sends a message to the registered receivers. 
        /// Usage: new TextMessage("Test").Send();
        /// Returns the input message for chaining. </summary>
        /// <param name="message">The message to send. </param>
        public Task<CallbackMessageResult<TFirst, TSecond, TThird>> SendAsync<TFirst, TSecond, TThird>(CallbackMessage<TFirst, TSecond, TThird> message)
        {
            Send(message);
            return message.Task;
        }
	}

    internal class MessageRegistration
    {
        public object Receiver;
        public Type Type;
        public object Action;
    }
}
