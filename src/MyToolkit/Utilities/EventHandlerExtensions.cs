//-----------------------------------------------------------------------
// <copyright file="EventHandlerExtensions.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace MyToolkit.Utilities
{
    /// <summary>Provides extension methods for event handlers. </summary>
    public static class EventHandlerExtensions
    {
        /// <summary>Raises the event in a thead-safe manner. </summary>
        /// <param name="handler">The event handler. </param>
        /// <param name="sender">The sender</param>
        /// <param name="args">The event arguments</param>
#if !LEGACY
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void Raise(this EventHandler handler, object sender, EventArgs args)
        {
            var copy = handler;
            if (copy != null)
                copy(sender, args);
        }

        /// <summary>Raises the event in a thead-safe manner. </summary>
        /// <typeparam name="TEventArgs">The type of the event arguments. </typeparam>
        /// <param name="handler">The event handler. </param>
        /// <param name="sender">The sender</param>
        /// <param name="args">The event arguments</param>
#if !LEGACY
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void Raise<TEventArgs>(this EventHandler<TEventArgs> handler, object sender, TEventArgs args)
            where TEventArgs : EventArgs
        {
            var copy = handler;
            if (copy != null)
                copy(sender, args);
        }

        /// <summary>Raises the event in a thead-safe manner. </summary>
        /// <typeparam name="TEventHandler">The type of the event handler. </typeparam>
        /// <typeparam name="TEventArgs">The type of the event arguments. </typeparam>
        /// <param name="handler">The event handler. </param>
        /// <param name="sender">The sender</param>
        /// <param name="args">The event arguments</param>
#if !LEGACY
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void Raise<TEventHandler, TEventArgs>(this TEventHandler handler, object sender, TEventArgs args)
            where TEventArgs : EventArgs
            where TEventHandler : class
        {
            var copy = handler;
            if (copy != null)
            {
#if !LEGACY
                var info = copy.GetType().GetRuntimeMethod("Invoke", new[] { typeof(object), typeof(TEventArgs) });
#else
                var info = copy.GetType().GetMethod("Invoke", new[] { typeof(object), typeof(TEventArgs) });
#endif
                info.Invoke(copy, new[] { sender, args });
            }
        }
    }
}
