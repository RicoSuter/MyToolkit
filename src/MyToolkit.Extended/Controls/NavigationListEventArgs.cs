//-----------------------------------------------------------------------
// <copyright file="NavigationListEventArgs.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;

namespace MyToolkit.Controls
{
    public class NavigationListEventArgs : EventArgs
    {
        internal NavigationListEventArgs(object item)
        {
            Item = item;
        }

        /// <summary>Gets the item to navigate to. </summary>
        public object Item { private set; get; }

        /// <summary>Gets the item to naviate to and casts it to the given generic type. </summary>
        /// <typeparam name="T">The item type. </typeparam>
        /// <returns>The item. </returns>
        public T GetItem<T>()
        {
            return (T)Item;
        }
    }
}