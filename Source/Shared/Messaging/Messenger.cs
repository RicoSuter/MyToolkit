using System;
using System.Collections.Generic;
using System.Linq;

#if WINRT || WP8
using System.Threading.Tasks;
#endif

namespace MyToolkit.Messaging
{
	class MessageDescriptor
	{
		public object Receiver;
		public Type Type;
		public object Action; 
	}

	public static class MessengerExtensions
	{
		/// <summary>
		/// Usage: new TextMessage("Test").Send();
		/// Returns the input message for chaining. 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="msg"></param>
		public static T Send<T>(this T msg)
		{
			Messenger.Send(msg);
			return msg; 
		}

#if WINRT || WP8
		public static async Task<T> SendAsync<T>(this T msg)
		{
			await Messenger.SendAsync(msg);
			return msg; 
		}
#endif
	}

	public static class Messenger
	{
		private static readonly List<MessageDescriptor> actions = new List<MessageDescriptor>();
		
		/// <summary>
		/// Registers an action for the given receiver. WARNING: You have to unregister the action to avoid memory leaks!
		/// </summary>
		/// <typeparam name="T">Type of the message</typeparam>
		/// <param name="receiver">Receiver to use as identifier</param>
		/// <param name="action">Action to register</param>
		public static void Register<T>(object receiver, Action<T> action)
		{
			actions.Add(new MessageDescriptor { Receiver = receiver, Type = typeof(T), Action = action});
		}

		/// <summary>
		/// Registers an action for no receiver. 
		/// </summary>
		/// <typeparam name="T">Type of the message</typeparam>
		/// <param name="action">Action to register</param>
		public static void Register<T>(Action<T> action)
		{
			Register(null, action);
		}

#if WINRT || WP8
		/// <summary>
		/// Registers an async action for the given receiver. WARNING: You have to unregister the action to avoid memory leaks!
		/// </summary>
		/// <typeparam name="T">Type of the message</typeparam>
		/// <param name="receiver">Receiver to use as identifier</param>
		/// <param name="action">Action to register</param>
		public static void RegisterTask<T>(object receiver, Func<T, Task> action)
		{
			actions.Add(new MessageDescriptor { Receiver = receiver, Type = typeof(T), Action = action });
		}

		/// <summary>
		/// Registers an async action for no receiver. 
		/// </summary>
		/// <typeparam name="T">Type of the message</typeparam>
		/// <param name="action">Action to register</param>
		public static void RegisterTask<T>(Func<T, Task> action)
		{
			RegisterTask(null, action);
		}
#endif

		/// <summary>
		/// Unregisters all actions with no receiver
		/// </summary>
		public static void Unregister()
		{
			Unregister(null);
		}

		/// <summary>
		/// Unregisters all actions with the given receiver
		/// </summary>
		/// <param name="receiver"></param>
		public static void Unregister(object receiver)
		{
			foreach (var a in actions.Where(a => a.Receiver == receiver).ToArray())
				actions.Remove(a);
		}

		/// <summary>
		/// Unregisters the specified action
		/// </summary>
		/// <typeparam name="T">Type of the message</typeparam>
		/// <param name="action">Action to unregister</param>
		public static void Unregister<T>(Action<T> action)
		{
			foreach (var a in actions.Where(a => (Action<T>)a.Action == action).ToArray())
				actions.Remove(a);
		}

		/// <summary>
		/// Unregisters the specified action
		/// </summary>
		/// <typeparam name="T">Type of the message</typeparam>
		public static void Unregister<T>()
		{
			foreach (var a in actions.Where(a => a.Type == typeof(T)).ToArray())
				actions.Remove(a);
		}

		/// <summary>
		/// Unregisters the specified action
		/// </summary>
		/// <param name="receiver"></param>
		/// <typeparam name="T">Type of the message</typeparam>
		public static void Unregister<T>(object receiver)
		{
			foreach (var a in actions.Where(a => a.Receiver == receiver && a.Type == typeof(T)).ToArray())
				actions.Remove(a);
		}

		/// <summary>
		/// Unregisters an action for the specified receiver. 
		/// </summary>
		/// <typeparam name="T">Type of the message</typeparam>
		/// <param name="receiver"></param>
		/// <param name="action"></param>
		public static void Unregister<T>(object receiver, Action<T> action)
		{
			foreach (var a in actions.Where(a => a.Receiver == receiver && (Action<T>)a.Action == action).ToArray())
				actions.Remove(a);
		}

		/// <summary>
		/// Sends a message to the registered receivers. 
		/// </summary>
		/// <typeparam name="T">Type of the message</typeparam>
		/// <param name="message"></param>
		public static T Send<T>(T message)
		{
			var type = typeof (T);
			foreach (var a in actions.Where(a => a.Type == type).ToArray())
			{
#if WINRT || WP8
				if (a.Action is Action<T>)
					((Action<T>)a.Action)(message);
				else
					((Func<T, Task>)a.Action)(message);
#else
				((Action<T>)a.Action)(message);
#endif
			}
			return message;
		}

#if WINRT || WP8
		/// <summary>
		/// Sends a message to the registered receivers. With this method it is possible to wait until async methods have finished. Async methods are called serially. 
		/// </summary>
		/// <typeparam name="T">Type of the message</typeparam>
		/// <param name="message"></param>
		public static async Task<T> SendAsync<T>(T message)
		{
			var type = typeof(T);
			foreach (var a in actions.Where(a => a.Type == type).ToArray())
			{
				if (a.Action is Action<T>)
					((Action<T>)a.Action)(message);
				else
					await ((Func<T, Task>)a.Action)(message);
			}
			return message;
		}
#endif
	}
}
