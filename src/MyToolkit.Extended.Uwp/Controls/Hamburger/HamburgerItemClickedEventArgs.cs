//-----------------------------------------------------------------------
// <copyright file="HamburgerItemClickedEventArgs.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;

namespace MyToolkit.Controls
{
    /// <summary>The hamburger item clicked event args.</summary>
    public class HamburgerItemClickedEventArgs : EventArgs
    {
        /// <summary>Initializes a new instance of the <see cref="HamburgerItemClickedEventArgs"/> class.</summary>
        /// <param name="hamburger">The hamburger.</param>
        public HamburgerItemClickedEventArgs(Hamburger hamburger)
        {
            Hamburger = hamburger;
        }

        /// <summary>Gets the clicked hamburger item.</summary>
        public Hamburger Hamburger { get; private set; }
    }
}