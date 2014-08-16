//-----------------------------------------------------------------------
// <copyright file="IMessenger.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>http://mytoolkit.codeplex.com/license</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Threading.Tasks;

namespace MyToolkit.Messaging
{
    /// <summary>
    /// The interface of the messenger. 
    /// </summary>
    public interface IMessenger
    {
        /// <summary>
        /// Registers an action for the given receiver. WARNING: You have to unregister the action to avoid memory leaks!
        /// </summary>
        /// <typeparam name="T">Type of the message</typeparam>
        /// <param name="receiver">Receiver to use as identifier</param>
        /// <param name="action">Action to register</param>
        void Register<T>(object receiver, Action<T> action);

        /// <summary>
        /// Registers an action for no receiver. 
        /// </summary>
        /// <typeparam name="T">Type of the message</typeparam>
        /// <param name="action">Action to register</param>
        void Register<T>(Action<T> action);

        /// <summary>
        /// Unregisters all actions with no receiver. 
        /// </summary>
        void Unregister();

        /// <summary>
        /// Unregisters all actions with the given receiver
        /// </summary>
        /// <param name="receiver"></param>
        void Unregister(object receiver);

        /// <summary>
        /// Unregisters the specified action
        /// </summary>
        /// <typeparam name="T">Type of the message</typeparam>
        /// <param name="action">Action to unregister</param>
        void Unregister<T>(Action<T> action);

        /// <summary>
        /// Unregisters the specified action
        /// </summary>
        /// <typeparam name="T">Type of the message</typeparam>
        void Unregister<T>();

        /// <summary>
        /// Unregisters the specified action
        /// </summary>
        /// <param name="receiver"></param>
        /// <typeparam name="T">Type of the message</typeparam>
        void Unregister<T>(object receiver);

        /// <summary>
        /// Unregisters an action for the specified receiver. 
        /// </summary>
        /// <typeparam name="T">Type of the message</typeparam>
        /// <param name="receiver"></param>
        /// <param name="action"></param>
        void Unregister<T>(object receiver, Action<T> action);

        /// <summary>
        /// Sends a message to the registered receivers. 
        /// </summary>
        /// <typeparam name="T">Type of the message</typeparam>
        /// <param name="message"></param>
        T Send<T>(T message);

        /// <summary>
        /// Sends a message to the registered receivers. 
        /// Usage: new TextMessage("Test").Send();
        /// Returns the input message for chaining. 
        /// </summary>
        /// <param name="msg">The message to send. </param>
        Task SendAsync(CallbackMessage msg);

        /// <summary>
        /// Usage: new TextMessage("Test").Send();
        /// Returns the input message for chaining. 
        /// </summary>
        /// <param name="msg">The message to send. </param>
        Task<CallbackMessageResult<T>> SendAsync<T>(CallbackMessage<T> msg);

        /// <summary>
        /// Sends a message to the registered receivers. 
        /// Usage: new TextMessage("Test").Send();
        /// Returns the input message for chaining. 
        /// </summary>
        /// <param name="msg">The message to send. </param>
        Task<CallbackMessageResult<TFirst, TSecond>> SendAsync<TFirst, TSecond>(CallbackMessage<TFirst, TSecond> msg);

        /// <summary>
        /// Sends a message to the registered receivers. 
        /// Usage: new TextMessage("Test").Send();
        /// Returns the input message for chaining. 
        /// </summary>
        /// <param name="msg">The message to send. </param>
        Task<CallbackMessageResult<TFirst, TSecond, TThird>> SendAsync<TFirst, TSecond, TThird>(CallbackMessage<TFirst, TSecond, TThird> msg);
    }
}