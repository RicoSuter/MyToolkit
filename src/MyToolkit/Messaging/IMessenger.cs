//-----------------------------------------------------------------------
// <copyright file="IMessenger.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Threading.Tasks;

namespace MyToolkit.Messaging
{
    /// <summary>The interface of the messenger. </summary>
    public interface IMessenger
    {
        /// <summary>Registers an action for the given receiver. WARNING: You have to deregister the action to avoid memory leaks! </summary>
        /// <typeparam name="T">Type of the message</typeparam>
        /// <param name="receiver">Receiver to use as identifier</param>
        /// <param name="action">Action to register</param>
        void Register<T>(object receiver, Action<T> action);

        /// <summary>Registers an action for no receiver. </summary>
        /// <typeparam name="T">Type of the message</typeparam>
        /// <param name="action">Action to register</param>
        void Register<T>(Action<T> action);

        /// <summary>Deregisters all actions with no receiver. </summary>
        void Deregister();

        /// <summary>Deregisters all actions with the given receiver. </summary>
        /// <param name="receiver"></param>
        void Deregister(object receiver);

        /// <summary>Deregisters the specified action. </summary>
        /// <typeparam name="T">Type of the message</typeparam>
        /// <param name="action">Action to deregister</param>
        void Deregister<T>(Action<T> action);

        /// <summary>Deregisters the specified action. </summary>
        /// <typeparam name="T">Type of the message</typeparam>
        void Deregister<T>();

        /// <summary>Deregisters the specified action. </summary>
        /// <param name="receiver"></param>
        /// <typeparam name="T">Type of the message</typeparam>
        void Deregister<T>(object receiver);

        /// <summary>Deregisters an action for the specified receiver. </summary>
        /// <typeparam name="T">Type of the message</typeparam>
        /// <param name="receiver"></param>
        /// <param name="action"></param>
        void Deregister<T>(object receiver, Action<T> action);

        /// <summary>Sends a message to the registered receivers. </summary>
        /// <typeparam name="T">Type of the message</typeparam>
        /// <param name="message"></param>
        T Send<T>(T message);

        /// <summary>Sends a message to the registered receivers. 
        /// Usage: new TextMessage("Test").Send();
        /// Returns the input message for chaining. </summary>
        /// <param name="msg">The message to send. </param>
        Task SendAsync(CallbackMessage msg);

        /// <summary>Usage: new TextMessage("Test").Send();
        /// Returns the input message for chaining. </summary>
        /// <param name="msg">The message to send. </param>
        Task<CallbackMessageResult<T>> SendAsync<T>(CallbackMessage<T> msg);

        /// <summary>Sends a message to the registered receivers. 
        /// Usage: new TextMessage("Test").Send();
        /// Returns the input message for chaining. </summary>
        /// <param name="msg">The message to send. </param>
        Task<CallbackMessageResult<TFirst, TSecond>> SendAsync<TFirst, TSecond>(CallbackMessage<TFirst, TSecond> msg);

        /// <summary>Sends a message to the registered receivers. 
        /// Usage: new TextMessage("Test").Send();
        /// Returns the input message for chaining. </summary>
        /// <param name="msg">The message to send. </param>
        Task<CallbackMessageResult<TFirst, TSecond, TThird>> SendAsync<TFirst, TSecond, TThird>(CallbackMessage<TFirst, TSecond, TThird> msg);
    }
}