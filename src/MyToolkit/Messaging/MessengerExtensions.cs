//-----------------------------------------------------------------------
// <copyright file="MessengerExtensions.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System.Threading.Tasks;

namespace MyToolkit.Messaging
{
    /// <summary>Extension methods to send messages using the default messenger. </summary>
    public static class MessengerExtensions
    {
        /// <summary>Sends a message to the registered receivers using the default messenger. </summary>
        /// <typeparam name="T">Type of the message. </typeparam>
        /// <param name="message">The message to send. </param>
        public static T Send<T>(this T message)
        {
            return Messenger.Default.Send(message);
        }

        /// <summary>Sends a message to the registered receivers using the default messenger. 
        /// Usage: new TextMessage("Test").Send();
        /// Returns the input message for chaining. </summary>
        /// <param name="message">The message to send. </param>
        public static Task SendAsync(this CallbackMessage message)
        {
            Send(message);
            return message.Task;
        }

        /// <summary>Sends a message to the registered receivers using the default messenger. 
        /// Usage: new TextMessage("Test").Send();
        /// Returns the input message for chaining. </summary>
        /// <param name="message">The message to send. </param>
        public static Task<CallbackMessageResult<T>> SendAsync<T>(this CallbackMessage<T> message)
        {
            Send(message);
            return message.Task;
        }

        /// <summary>Sends a message to the registered receivers using the default messenger. 
        /// Usage: new TextMessage("Test").Send();
        /// Returns the input message for chaining. </summary>
        /// <param name="message">The message to send. </param>
        public static Task<CallbackMessageResult<TFirst, TSecond>> SendAsync<TFirst, TSecond>(this CallbackMessage<TFirst, TSecond> message)
        {
            Send(message);
            return message.Task;
        }

        /// <summary>Sends a message to the registered receivers using the default messenger. 
        /// Usage: new TextMessage("Test").Send();
        /// Returns the input message for chaining. </summary>
        /// <param name="message">The message to send. </param>
        public static Task<CallbackMessageResult<TFirst, TSecond, TThird>> SendAsync<TFirst, TSecond, TThird>(this CallbackMessage<TFirst, TSecond, TThird> message)
        {
            Send(message);
            return message.Task;
        }
    }
}