//-----------------------------------------------------------------------
// <copyright file="HamburgerItemChangedEventArgs.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;

namespace MyToolkit.Controls
{
    /// <summary>The hamburger item changed event args.</summary>
    public class HamburgerItemChangedEventArgs : EventArgs
    {
        /// <summary>Initializes a new instance of the <see cref="HamburgerItemChangedEventArgs"/> class.</summary>
        /// <param name="item">The item.</param>
        public HamburgerItemChangedEventArgs(HamburgerItem item)
        {
            Item = item;
        }

        /// <summary>Gets the currently selected hamburger item.</summary>
        public HamburgerItem Item { get; private set; }
    }
}