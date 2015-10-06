//-----------------------------------------------------------------------
// <copyright file="HamburgerItemSelectedEventArgs.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;

namespace MyToolkit.Controls
{
    /// <summary>The hamburger item selected event args.</summary>
    public class HamburgerItemSelectedEventArgs : EventArgs
    {
        /// <summary>Initializes a new instance of the <see cref="HamburgerItemSelectedEventArgs"/> class.</summary>
        /// <param name="hamburger">The hamburger control.</param>
        public HamburgerItemSelectedEventArgs(Hamburger hamburger)
        {
            Hamburger = hamburger;
        }

        /// <summary>Gets the parent hamburger control.</summary>
        public Hamburger Hamburger { get; private set; }
    }
}